using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;

namespace Kopis_Showcase.Models
{
    public interface IUploadFile
    {
        Person CreatePersonFromRow(IRow row);
        void ReadEachRowFromSheet(ISheet sheet);
        int GetGender(string GenderName);
        int GetMaritalStatus(string MaritalName);

    }
}
