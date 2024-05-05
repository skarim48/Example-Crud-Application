using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ModelProject;
using ModelProject.JWT;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelProject.Repo
{
    public class Repository
    {
        private Microsoft.AspNetCore.Identity.UserManager<IdentityUser> _userManager;
        public EFContext identityUserrepo = new EFContext();


        public void insertProduct(Product product)
        {
            using (var db = new EFContext())
            {
                db.Add(product);
                db.SaveChanges();
            }
            return;
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            using (var db = new EFContext())
            {
                return await db.Products.ToListAsync();
                
            }
        }
        public List<Product> readProduct()
        {
            List<Product> result= new List<Product>();
            using (var db = new EFContext())
            {
                List<Product> products = db.Products.ToList();
                foreach (Product p in products)
                {
                    result.Add(p);
                }
            }
            return result;
        }

        static void updateProduct()
        {
            using (var db = new EFContext())
            {
                Product product = db.Products.Find(1);
                product.Name = "Better Pen Drive";
                db.SaveChanges();
            }
            return;
        }

        public void deleteProduct(string IdProduct)
        {
            using (var db = new EFContext())
            {
                Product product = db.Products.Find(int.Parse(IdProduct));
                db.Products.Remove(product);
                db.SaveChanges();
            }
            return;
        }

        public Product getProduct(string IdProduct)
        {
            using (var db = new EFContext())
            {
                Product product = db.Products.Find(int.Parse(IdProduct));
                return product;

            }
        }

        public void updateeProduct(Product updateProduct)
        {
            using (var db = new EFContext())
            {
                db.Products.Update(updateProduct);
                db.SaveChanges();
            }
            return;
        }

        public async void CreateUser(IdentityUser user)
        {

        }

        public IdentityUser getCurrentUser(string currentToken)
        {
            try
            {
                JwtSecurityToken jwtSecurityToken = TokenManagement.ConvertJwtStringToJwtSecurityToken(currentToken);
                DecodedToken decodedToken = TokenManagement.DecodeJwt(jwtSecurityToken);
                string idUserCurrentUser = decodedToken.claims.Where(c => c.Type == "IdUser").FirstOrDefault().Value;
                IdentityUser currentUser = identityUserrepo.Users.Where(u => u.Id == idUserCurrentUser).FirstOrDefault();
                return currentUser;
            }
            catch (Exception ex)
            {
            }
            return null;
        }
    }

    public class TokenManagement
    {
        public static JwtSecurityToken ConvertJwtStringToJwtSecurityToken(string? jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);

            return token;
        }

        public static DecodedToken DecodeJwt(JwtSecurityToken token)
        {
            var keyId = token.Header.Kid;
            var audience = token.Audiences.ToList();
            var claims = token.Claims.Select(claim => (claim.Type, claim.Value)).ToList();
            return new DecodedToken(
                keyId,
                token.Issuer,
                audience,
                claims,
                token.ValidTo,
                token.SignatureAlgorithm,
                token.RawData,
                token.Subject,
                token.ValidFrom,
                token.EncodedHeader,
                token.EncodedPayload
            );
        }
    }

    public class DecodedToken
    {
        public string keyId;
        public string issuer;
        public List<string> audience;
        public List<(string Type, string Value)> claims;
        public DateTime validTo;
        public string signatureAlgorithm;
        public string rawData;
        public string subject;
        public DateTime validFrom;
        public string encodedHeader;
        public string encodedPayload;

        public DecodedToken(string keyId, string issuer, List<string> audience, List<(string Type, string Value)> claims, DateTime validTo, string signatureAlgorithm, string rawData, string subject, DateTime validFrom, string encodedHeader, string encodedPayload)
        {
            this.keyId = keyId;
            this.issuer = issuer;
            this.audience = audience;
            this.claims = claims;
            this.validTo = validTo;
            this.signatureAlgorithm = signatureAlgorithm;
            this.rawData = rawData;
            this.subject = subject;
            this.validFrom = validFrom;
            this.encodedHeader = encodedHeader;
            this.encodedPayload = encodedPayload;
        }
    }
}
