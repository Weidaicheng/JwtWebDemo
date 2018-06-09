namespace JwtWebDemo.Models
{
    public abstract class PayloadBase
    {
        /// <summary>
        /// 签发时间
        /// </summary>
        public long iat { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public long exp { get; set; }
    }

    /// <summary>
    /// Jwt载荷
    /// </summary>
    public class Payload : PayloadBase
    {
        public string OpenId { get; set; }
    }
}
