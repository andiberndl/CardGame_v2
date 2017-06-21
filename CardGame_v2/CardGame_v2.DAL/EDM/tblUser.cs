//------------------------------------------------------------------------------
// <auto-generated>
//     Der Code wurde von einer Vorlage generiert.
//
//     Manuelle Änderungen an dieser Datei führen möglicherweise zu unerwartetem Verhalten der Anwendung.
//     Manuelle Änderungen an dieser Datei werden überschrieben, wenn der Code neu generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CardGame_v2.DAL.EDM
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblUser
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblUser()
        {
            this.tblDeck = new HashSet<tblDeck>();
            this.tblUserCardCollection = new HashSet<tblUserCardCollection>();
            this.tblVirtualPurchase = new HashSet<tblVirtualPurchase>();
        }
    
        public int idUser { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public string userpassword { get; set; }
        public string usersalt { get; set; }
        public Nullable<int> fkUserRole { get; set; }
        public byte[] avatar { get; set; }
        public int currency { get; set; }
        public string street { get; set; }
        public Nullable<int> zipcode { get; set; }
        public string city { get; set; }
        public Nullable<System.DateTime> regdate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblDeck> tblDeck { get; set; }
        public virtual tblUserRole tblUserRole { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblUserCardCollection> tblUserCardCollection { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblVirtualPurchase> tblVirtualPurchase { get; set; }
    }
}
