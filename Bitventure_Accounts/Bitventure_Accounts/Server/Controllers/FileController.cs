using Bitventure_Accounts.Server.Context;
using Bitventure_Accounts.Shared.Domain;
using Bitventure_Accounts.Shared.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;

namespace Bitventure_Accounts.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment env;
        private readonly AccountsDbContext dbContext;

        public FileController(IWebHostEnvironment env, AccountsDbContext dbContext)
        {
            this.env = env;
            this.dbContext = dbContext;
        }

        [Route("Upload")]
        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] UploadFileCommand uploadedFile)
        {
            // FINISH this please
            
            // Check if thefile is there
            if (uploadedFile == null)
                return BadRequest("File is required");

            // Get the extension
            var extension = Path.GetExtension(uploadedFile.FileName);

            if (extension != ".csv")
                return BadRequest("Please upload a csv file.");

            //Convert string into a MemoryStream
            var stream = new MemoryStream(uploadedFile.FileContent);

            //Parse the stream
            using (TextFieldParser parser = new TextFieldParser(stream))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                int rowCount = 1, columnCount = 1;
                string paymentId = "", accountHolder = "", branchCode = "", accountNumber = "", accountType = "", transactionDate = "", amount = "", status = "", effectiveStatusDate = "";

                while (!parser.EndOfData)
                {
                    //Processing row
                    string[] row = parser.ReadFields();
                    if (rowCount <= 1) //Skip header row
                    {
                        rowCount++;
                        continue;
                    }
                    foreach (string field in row)
                    {
                        switch (columnCount) 
                        {
                            case 1:
                                paymentId = field;
                                // Try to convert AccountType to valid type
                                if (!int.TryParse(paymentId, out _))
                                    return BadRequest($"Payment Id in row {rowCount} is incorrect.");
                                break;
                            case 2:
                                accountHolder = field;
                                break;
                            case 3:
                                branchCode = field;
                                break;
                            case 4:
                                accountNumber = field;
                                break;
                            case 5:
                                accountType = field;
                                // Try to convert AccountType to valid type
                                try
                                {
                                    var _ = (AccountType)Convert.ToInt32(accountType);
                                }
                                catch (Exception error)
                                {
                                    return BadRequest($"Account type in row {rowCount} is incorrect.");
                                }
                                break;
                            case 6:
                                transactionDate = field;
                                // Try to convert TransactionDate to DateTime
                                if (!DateTime.TryParseExact(transactionDate,
                                        "dd/MM/yyyy",
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, 
                                        out _))
                                    return BadRequest($"Transaction Date in row {rowCount} is incorrect.");
                                break;
                            case 7:
                                amount = field;
                                if(!decimal.TryParse(amount.Replace(".", ","), out _))
                                    return BadRequest($"Amount in row {rowCount} is incorrect.");
                                break;
                            case 8:
                                status = field;
                                break;
                            case 9:
                                effectiveStatusDate = field;
                                // Try to convert EffectiveStatusDate to DateTime
                                if (!DateTime.TryParseExact(transactionDate,
                                        "dd/MM/yyyy",
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None,
                                        out _))
                                    return BadRequest($"Effective status date in row {rowCount} is incorrect.");
                                break;

                        }
                        columnCount++;
                    }
                    rowCount++;
                    if (!dbContext.Masters.Select(account => account.AccountNumber).Contains(accountNumber))
                    {
                        var newAccountRecord = new Master(accountHolder, branchCode, accountNumber, (AccountType)Convert.ToInt32(accountType));
                        dbContext.Masters.Add(newAccountRecord);
                        dbContext.SaveChanges();
                    }

                    var account = dbContext.Masters.SingleOrDefault(account => account.AccountNumber == accountNumber);
                    var newDetailRecord = new Detail(Convert.ToInt32(paymentId), DateTime.ParseExact(transactionDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None), decimal.Parse(amount.Replace(".", ",")), status, DateTime.ParseExact(effectiveStatusDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None), account);
                    dbContext.Details.Add(newDetailRecord);
                    dbContext.SaveChanges();
                    columnCount = 1;
                }
            }
            // You are done return the new URL which is (yourapplication url/data/newfilename)
            return Ok($"{uploadedFile.FileName} importedsSuccessfully");
        }
    }
}
