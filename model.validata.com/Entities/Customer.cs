using model.validata.com.ValueObjects.Customer;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using util.validata.com;

namespace model.validata.com.Entities
{
   
    [Table(nameof(Customer), Schema = Constants.DefaultSchema)]
    public class Customer : BaseEntity
    {
        
        public Customer() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerId { get; private set; } 

        
        [Required]
        [MaxLength(128)]
        public string FirstNameValue { get; private set; }

        [Required]
        [MaxLength(128)]
        [NotMapped] 
        public FirstName FirstName { get; private set; }

        [Required]
        [MaxLength(128)]
        public string LastNameValue { get; private set; }

        [Required]
        [MaxLength(128)]
        [NotMapped]
        public LastName LastName { get; private set; }

        [Required]
        [MaxLength(128)]
        public string EmailValue { get; private set; }

        [Required]
        [MaxLength(128)]
        [NotMapped]
        public EmailAddress Email { get; private set; }

        [Required]
        [MaxLength(512)]
        public string AddressValue { get; private set; }

        [Required]
        [MaxLength(512)]
        [NotMapped]
        public StreetAddress Address { get; private set; }

        [Required]
        [MaxLength(10)]
        public string PoboxValue { get; private set; }

        [Required]
        [MaxLength(10)]
        [NotMapped]
        public PostalCode Pobox { get; private set; }

        
        public Customer(
            int customerId,
            FirstName firstName,
            LastName lastName,
            EmailAddress email,
            StreetAddress address,
            PostalCode pobox)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Address = address ?? throw new ArgumentNullException(nameof(address));
            Pobox = pobox ?? throw new ArgumentNullException(nameof(pobox));

            FirstNameValue = firstName.Value;
            LastNameValue = lastName.Value;
            EmailValue = email.Value;
            AddressValue = address.Value;
            PoboxValue = pobox.Value;
            CustomerId = customerId;

            CreatedOnTimeStamp = DateTimeUtil.SystemTime;
            LastModifiedTimeStamp = DateTimeUtil.SystemTime;
        }

        public void UpdateFirstName(FirstName newFirstName)
        {
            FirstName = newFirstName ?? throw new ArgumentNullException(nameof(newFirstName));
            FirstNameValue = newFirstName.Value;

        }

        public void UpdateLastName(LastName newLastName)
        {
            LastName = newLastName ?? throw new ArgumentNullException(nameof(newLastName));
            LastNameValue = newLastName.Value;

        }

        public void UpdateAddress(StreetAddress newAddress)
        {
            Address = newAddress ?? throw new ArgumentNullException(nameof(newAddress));
            AddressValue = newAddress.Value;

        }

        public void UpdatePobox(PostalCode newPobox)
        {
            Pobox = newPobox ?? throw new ArgumentNullException(nameof(newPobox));
            PoboxValue = newPobox.Value;
        }

        public void UpdateEmail(EmailAddress newEmail)
        {
            
            Email = newEmail ?? throw new ArgumentNullException(nameof(newEmail));
            EmailValue = newEmail.Value;

        }

       
       

        
        public void LoadValueObjectsFromBackingFields()
        {
            if (FirstName == null! && FirstNameValue != null)
            {
                FirstName = new FirstName(FirstNameValue);
            }
            if (LastName == null! && LastNameValue != null)
            {
                LastName = new LastName(LastNameValue);
            }
            if (Email == null! && EmailValue != null)
            {
                Email = new EmailAddress(EmailValue);
            }
            if (Address == null! && AddressValue != null)
            {
                Address = new StreetAddress(AddressValue);
            }
            if (Pobox == null! && PoboxValue != null)
            {
                Pobox = new PostalCode(PoboxValue);
            }
        }
    }
}
