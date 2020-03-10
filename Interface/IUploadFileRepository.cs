using NPOI.SS.UserModel;
using Kopis_Showcase.Models;

namespace Kopis_Showcase.Interface
{
    public interface IUploadFileRepository
    {
        Person CreatePersonFromRow(IRow row);
        void ReadEachRowFromSheet(ISheet sheet);
        int GetGender(string GenderName);
        int GetMaritalStatus(string MaritalName);

    }
}
