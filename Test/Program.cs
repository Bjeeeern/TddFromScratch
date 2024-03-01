using System.Text;
using Product;

Console.OutputEncoding = Encoding.UTF8;

var userRegister = new UserRegister();

if (userRegister.Users.Count != 0) throw new Exception();

userRegister.RegisterUser("test.testsson", "qwerty123456");

if (userRegister.Users.Count != 1) throw new Exception();

Console.WriteLine("✅ All tests OK ✅");