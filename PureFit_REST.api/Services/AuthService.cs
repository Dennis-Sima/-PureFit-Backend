using Microsoft.IdentityModel.Tokens;
using PureFit_REST.api.Model;
using PureFit_REST.Dto;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PureFit_REST.api.Services
{
    /// <summary>
    /// TODO: Die Methode CheckUserAndGetRole() anpassen. Eventuell ist noch der DbContext
    /// im Konstruktor zu ergänzen, damit das Service Zugriff auf die Datenbank hat.
    /// </summary>
    public class AuthService
    {
        
        private readonly byte[] _secret = new byte[0];

        /// <summary>
        /// Konstruktor für die Verwendung ohne JWT.8
        /// </summary>
        public AuthService()
        {        
   
        }

        /// <summary>
        /// Konstruktor mit Secret für die Verwendung mit JWT.
        /// </summary>
        /// <param name="secret">Base64 codierter String für das Secret des JWT.</param>
        public AuthService(string secret)
        {
            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentException("Secret is null or empty.", nameof(secret));
            }
            _secret = Convert.FromBase64String(secret);

        }
        /// <summary>
        /// Erstellt einen neuen Benutzer in der Datenbank. Dafür wird ein Salt generiert und der
        /// Hash des Passwortes berechnet.

        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>     
        public async Task<User> CreateUser(UserDto credentials, PureFitDbContext con)
        {

            string salt = GenerateRandom();
           
            User newUser = new User
            {
                U_Name = credentials.Username,
                U_Salt = salt,
                U_Hash = CalculateHash(credentials.Password, salt),
                U_Kunde_Nr =null,
                U_Role = "Admin",   //Alle die sich registrieren sind Kunden und keine Admins zum bearbeiten
                U_ID = null        //ID muss zuerst auf null gesetzt werden, weil db sonst nicht erkennt, dass AutoIncrement gesetzt werden soll!


            };

            con.Entry(newUser).State = Microsoft.EntityFrameworkCore.EntityState.Added;
            await con.SaveChangesAsync();
            return newUser;
        }


        /// <summary>
        /// Erstellt einen neuen Benutzer in der Datenbank. Dafür wird ein Salt generiert und der
        /// Hash des Passwortes berechnet.

        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>     
        public async Task<User> CreateUserKunde(UserKundenDto credentials, PureFitDbContext con)
        {
           
                string salt = GenerateRandom();
                // Den neuen Userdatensatz erstellen
                Kunde newKunde = new Kunde
                {
                    K_Vorname = credentials.Vorname,
                    K_Zuname = credentials.Zuname,
                    K_Geschlecht = credentials.Geschlecht,
                    K_GebDatum = Encoding.ASCII.GetBytes(credentials.GebDatum),
                    K_Gewicht = Encoding.ASCII.GetBytes(credentials.Gewicht),
                    K_Groesse = Encoding.ASCII.GetBytes(credentials.Groesse),
                    K_TelefonNr = credentials.TelefonNr,
                    K_Email = credentials.Email,
                    K_Trainingslevel = con.Trainingslevel.Where(w => w.tr_levelname == credentials.Trainingslevel).Select(s => s.tr_levelNr).FirstOrDefault(),
                    K_Nr = null
                };

            con.Entry(newKunde).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                await con.SaveChangesAsync();
        
                User newUser = new User
                {
                    U_Name = credentials.Username,
                    U_Salt = salt,
                    U_Hash = CalculateHash(credentials.Password, salt),
                    U_Kunde_Nr = newKunde.K_Nr,
                    U_Role = "Kunde",   //Alle die sich registrieren sind Kunden und keine Admins zum bearbeiten
                    U_ID = null        //ID muss zuerst auf null gesetzt werden, weil db sonst nicht erkennt, dass AutoIncrement gesetzt werden soll!
                    
                
                };

             con.Entry(newUser).State = Microsoft.EntityFrameworkCore.EntityState.Added;
               await con.SaveChangesAsync();
                return newUser;
        }
        /// <summary>
        /// Generiert den JSON Web Token für den übergebenen User.
        /// </summary>
        /// <param name="credentials">Userdaten, die in den Token codiert werden sollen.</param>
        /// <returns>
        /// JSON Web Token, wenn der User Authentifiziert werden konnte. 
        /// Null wenn der Benutzer nicht gefunden wurde.
        /// </returns>
        public async Task<string> GenerateToken(UserDto credentials, TimeSpan lifetime, PureFitDbContext con)
        {
            if (credentials is null) 
                throw new ArgumentNullException(nameof(credentials)); 

            string role = await CheckUserAndGetRole(credentials, con);
            if (role == null) 
                return null; 

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                // Payload für den JWT.
                Subject = new ClaimsIdentity(new Claim[]
                {
                    // Benutzername als Typ ClaimTypes.Name.
                    new Claim(ClaimTypes.Name, credentials.Username.ToString()),
                    // Rolle des Benutzer als ClaimTypes.DefaultRoleClaimType
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
                }),
                Expires = DateTime.UtcNow + lifetime,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(_secret),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Prüft, ob der übergebene User existiert und gibt seine Rolle zurück.
        /// TODO: Anpassen der Logik an die eigenen Erfordernisse.
        /// </summary>
        /// <param name="credentials">Benutzername und Passwort, die geprüft werden.</param>
        /// <returns>
        /// Rolle, wenn der Benutzer authentifiziert werden konnte.
        /// Null, wenn der Benutzer nicht authentifiziert werden konnte.
        /// </returns>
        protected virtual async Task<string> CheckUserAndGetRole(UserDto credentials, PureFitDbContext con)
        {
            // Abfrage, ob es den User überhaupt gibt
            if (!con.User.Any(u => u.U_Name == credentials.Username)) 
                return null;

            //Lese das salt aus der DB vom User
            string dbsalt = con.User.Where(u => u.U_Name == credentials.Username).Select(w => w.U_Salt).FirstOrDefault();
            //Lese den hash aus der DB vom User
            string dbHash = con.User.Where(u => u.U_Name == credentials.Username).Select(w => w.U_Hash).FirstOrDefault();

            // TODO: Um das Passwort zu prüfen, berechnen wir den Hash mit dem Salt in der DB. Stimmt
            // das Ergebnis mit dem gespeichertem Ergebnis überein, ist das Passwort richtig.
            string hash = CalculateHash(credentials.Password, dbsalt); 
            if (hash != dbHash) 
                return null;

            // die Rolle zuweisen
            return con.User.Where(u => u.U_Name == credentials.Username).Select(w => w.U_Role).FirstOrDefault();
        }







        //-------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Generiert eine Zufallszahl und gibt sie Base64 codiert zurück.
        /// </summary>
        /// <returns></returns>
        public static string GenerateRandom(int length = 128)
        {
            // Salt erzeugen.
            byte[] salt = new byte[length / 8];
            using (System.Security.Cryptography.RandomNumberGenerator rnd =
                System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rnd.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        /// <summary>
        /// Berechnet den HMACSHA256 Wert des Passwortes mit dem übergebenen Salt.
        /// </summary>
        /// <param name="password">Base64 Codiertes Passwort.</param>
        /// <param name="salt">Base64 Codiertes Salt.</param>
        /// <returns></returns>
        protected static string CalculateHash(string password, string salt)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(salt))
            {
                throw new ArgumentException("Invalid Salt or Passwort.");
            }
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

            System.Security.Cryptography.HMACSHA256 myHash =
                new System.Security.Cryptography.HMACSHA256(saltBytes);

            byte[] hashedData = myHash.ComputeHash(passwordBytes);

            // Das Bytearray wird als Hexstring zurückgegeben.
            string hashedPassword = Convert.ToBase64String(hashedData);
            return hashedPassword;
        }
        
    }

}