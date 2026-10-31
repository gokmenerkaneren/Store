namespace Store.Basket.LoginServices
{
    public class LoginService : ILoginService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        //public string GetUserId => _httpContextAccessor.HttpContext.User.FindFirst("sub").Value;
        public Guid GetUserId => Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst("sub")?.Value ?? throw new InvalidOperationException("User ID not found"));
        // Sub'ın içerisinde kullanıcının Id'si var. Sub da tokenden gelecek.
        // Basketteki userId ile eşleşen bir değer var mı onu kontrol etmem lazım. Eğer yoksa hata fırlatmam lazım.
    }
}

