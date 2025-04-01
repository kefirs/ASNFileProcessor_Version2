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
                    string[] lines = File.ReadAllLines(filePath);
                    ASNHeader currentHeader = null;

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("HDR"))
                        {
                            currentHeader = new ASNHeader
                            {
                                SupplierId = line.Substring(4, 8).Trim(), // First 7 simbols after HDR (Supplier ID)
                                BoxId = line.Substring(97, 8).Trim(), // Next symbols after Supplier
                            };

                            _context.ASNHeaders.Add(currentHeader); // Adds new box to the database
                            _context.SaveChanges(); // Saves changes to the database
                        }
                        else if (line.StartsWith("LINE") && currentHeader != null)
                        {
                            var productLine = new ASNLine
                            {
                                ProductId = line.Substring(5, 10).Trim(), // Product ID i.e "P000001661"
                                ISBNCode = line.Substring(42, 13).Trim(), // Product code i.e "9781473663800"
                                Quantity = int.TryParse(line.Substring(76, 2).Trim(), out int quantity) ? quantity : (int?)null, // Quantity of the product
                                ASNHeaderId = currentHeader.Id
                            };

                            _context.ASNLines.Add(productLine); // Adds new product to the database
                            Console.WriteLine($"ProductId: {productLine.ProductId}, ISBNCode: {productLine.ISBNCode}, Quantity: {productLine.Quantity}, HeaderId: {currentHeader.Id}");
                            _context.SaveChanges(); // Saves changes to the database
                        }
                    }
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
