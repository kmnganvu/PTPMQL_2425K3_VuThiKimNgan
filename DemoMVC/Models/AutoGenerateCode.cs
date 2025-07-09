namespace DemoMVC.Models
{
    // Lớp này sẽ được sử dụng để tự động sinh mã cho các lớp khác trong dự án.
    public class AutoGenerateCode
    {
        // Các thuộc tính và phương thức có thể được thêm vào đây để hỗ trợ việc tự động sinh mã.
        // Ví dụ: Tạo các phương thức để sinh mã cho các lớp Entity Framework, Controller, View, v.v.
        public string GenerateCode(string id)
        {
            if (string.IsNullOrEmpty(id))// Kiểm tra xem mã có rỗng hoặc null không
            {
                return "PS001"; // Nếu không có mã cũ, trả về mã mặc định.
            }
            string prefix = new string(id.TakeWhile(char.IsLetter).ToArray()); // Lấy tiền tố từ mã cũ;
            string numberPart = new string(id.SkipWhile(char.IsLetter).ToArray()); // Lấy phần số từ mã cũ;
            
            if (int.TryParse(numberPart, out int number))
            {
                // Nếu phần số có thể chuyển đổi sang int, tăng số lên 1 và trả về mã mới.
                number++; // Tăng số lên 1
                return prefix + number.ToString("D3"); // Trả về mã mới với định dạng 3 chữ số
            }
            return "PS001"; // Nếu không thể chuyển đổi, trả về mã mặc định.
        }
    }
}