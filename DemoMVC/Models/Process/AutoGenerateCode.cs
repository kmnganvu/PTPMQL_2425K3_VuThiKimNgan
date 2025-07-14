namespace DemoMVC.Models.Process
{
    // Lớp này sẽ được sử dụng để tự động sinh mã cho các lớp khác trong dự án.
    public class AutoGenerateCode
    {
        // Các thuộc tính và phương thức có thể được thêm vào đây để hỗ trợ việc tự động sinh mã.
        // Ví dụ: Tạo các phương thức để sinh mã cho các lớp Entity Framework, Controller, View, v.v.
        public string GenerateCode(string inputID, int prefixLength)
        {
            string strOutput = "";
            //lay phan text cua inputID
            string prefix = inputID.Substring(0, prefixLength);
            //lay phan so cua inputID
            string numberPart = inputID.Substring(prefixLength);
            //chuyen so thanh so nguyen
            int number = int.Parse(numberPart);
            //tang so len 1 don vi
            number++;
            //chuyen so ve chuoi
            strOutput = prefix + number.ToString();
            return strOutput;
        }
        public string GenerateId(string inputID)
        {
            //STD008
            var match = System.Text.RegularExpressions.Regex.Match(inputID, @"^(?<prefix>[A-Za-z]+)(?<number>\d+)$");
            if (!match.Success)
            {
                throw new ArgumentException("Invalid id format");
            }
            string prefix = match.Groups["prefix"].Value;
            //STD
            string numberPart = match.Groups["number"].Value;
            //008
            int number = int.Parse(numberPart) + 1;
            //9
            string newNumberPart = number.ToString().PadLeft(numberPart.Length, '0');
            //STD009
            return prefix + newNumberPart;
        }
    }
}