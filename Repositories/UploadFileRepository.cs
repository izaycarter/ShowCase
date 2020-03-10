using System;
using System.IO;
using Kopis_Showcase.Data;
using Microsoft.AspNetCore.Hosting;
using Kopis_Showcase.Interface;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Kopis_Showcase.Models;

namespace Kopis_Showcase.Repositories
{
    public class UploadFileRepository : IUploadFileRepository
    {
        private IWebHostEnvironment _env;
        private readonly SqlDbContext context;

        public UploadFileRepository(IWebHostEnvironment env, SqlDbContext context)
        {
            _env = env;
            this.context = context;
        }

        public int GetGender(string GenderName)
        {
            switch (GenderName)
            {
                case "FEMALE":
                    return 1;
                case "MALE":
                    return 2;
                default:
                    return 3;

            }
        }
         

        public int GetMaritalStatus(string MaritalName)
        {
            switch (MaritalName)
            {
                case "SINGLE":
                    return 1;
                case "MARRIED":
                    return 2;
                case "DIVORCED":
                    return 3;
                case "WIDOWED":
                    return 4;
                default:
                    return 5;

            }
        }




        public Person CreatePersonFromRow(IRow row)
        {
            var person = new Person();

            for (int i = row.FirstCellNum; i <= row.LastCellNum; i++)
            {
                ICell cell = row.GetCell(i);
                switch (i)
                {
                    case 0:
                        person.FirstName = cell.ToString();
                        break;
                    case 1:
                        person.LastName = cell.ToString();
                        break;
                    case 2:
                        person.GenderID = GetGender(cell.ToString().ToUpper());
                        break;
                    case 3:
                        DateTime date = DateTime.Parse(cell.ToString());
                        person.DateOfBirth = date;
                        break;
                    case 4:
                        person.MaritalStatusID = GetMaritalStatus(cell.ToString().ToUpper());
                        break;
                    case 5:
                        person.EmailAddress = cell.ToString();
                        break;
                    case 6:
                        person.PhoneNumber = cell.ToString();
                        break;
                    case 7:
                        person.StreetAddressLine1 = cell.ToString();
                        break;
                    case 8:
                        person.StreetAddressLine2 = cell.ToString();
                        break;
                    case 9:
                        person.City = cell.ToString();
                        break;
                    case 10:
                        person.State = cell.ToString();
                        break;
                    case 11:
                        person.Zip = cell.ToString();
                        break;

                }
                context.Persons.AddAsync(person);
            }
            return person;

        }

        public void ReadEachRowFromSheet(ISheet sheet)
        {
            for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                CreatePersonFromRow(row);
                
                
            }
            context.SaveChanges();
            
        }

        public void GetFileAndRead(UploadFile file)
        {
            
            var filepath = Path.Combine(_env.ContentRootPath, "wwwroot/uploads", file.Upload.FileName);
            using (var fileStream = new FileStream(filepath, FileMode.Create))
            {
                 file.Upload.CopyToAsync(fileStream);
            }
            XSSFWorkbook workbook;

            using (var stream = new FileStream(filepath, FileMode.Open, FileAccess.ReadWrite))
            {
                file.Upload.CopyToAsync(stream);
                stream.Position = 0;
                workbook = new XSSFWorkbook(stream);
            }
            ISheet sheet = workbook.GetSheetAt(0);

            ReadEachRowFromSheet(sheet);

        }
    }
}