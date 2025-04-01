using System.Diagnostics.Eventing.Reader;

namespace ASNFileProcessor
{

    /// <summary>
    /// File.ReadAllLines(filePath); reads all file lines, If it finds HDR line, it creates a new box in the database. Same with Line, if it finds LINE line, it creates a new product in the database.
    /// </summary>
    public class ASNFileProcessor
    {
        private readonly ShippingDbContext _context;

        public ASNFileProcessor(ShippingDbContext context)
        {
            _context = context;
        }

        public void ProcessFile(string filePath)
        {
            int retryCount = 5;
            int delayMilliseconds = 1000;
            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    List<ASNHeader> headersToAdd = new List<ASNHeader>();
                    List<ASNLine> linesToAdd = new List<ASNLine>();
                    ASNHeader currentHeader = null;

                    using (var reader = new StreamReader(filePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line.StartsWith("HDR"))
                            {
                                currentHeader = new ASNHeader
                                {
                                    SupplierId = line.Substring(4, 8).Trim(),
                                    BoxId = line.Substring(97, 8).Trim(),
                                };

                                headersToAdd.Add(currentHeader);
                            }
                            else if ((line.StartsWith("LINE") && currentHeader != null))
                            {
                                var productLine = new ASNLine
                                {
                                    ProductId = line.Substring(5, 10).Trim(),
                                    ISBNCode = line.Substring(42, 13).Trim(),
                                    Quantity = int.TryParse(line.Substring(76, 2).Trim(), out int quantity) ? quantity : (int?)null,
                                    ASNHeaderId = currentHeader.Id
                                };

                                linesToAdd.Add(productLine);
                            }
                        }
                    }

                    _context.ASNHeaders.AddRange(headersToAdd);
                    _context.ASNLines.AddRange(linesToAdd);
                    _context.SaveChanges();  // Pielāgo visas izmaiņas vienā reizē

                    break;
                }
                catch (IOException ex)
                {
                    if (i == retryCount - 1)
                    {
                        Console.WriteLine($"Unable to access the file after {retryCount} attempts: {ex.Message}");
                        throw;
                    }

                    Console.WriteLine($"File is in use, retrying. Attempt: {i + 1}/{retryCount}");
                    Thread.Sleep(delayMilliseconds);
                }
            }
        }
    }
}
