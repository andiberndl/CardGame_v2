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
    
    public partial class tblCardPack
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblCardPack()
        {
            this.tblVirtualPurchase = new HashSet<tblVirtualPurchase>();
        }
    
        public int idCardPack { get; set; }
        public string packname { get; set; }
        public int numcards { get; set; }
        public int packprice { get; set; }
        public Nullable<int> RubyAmount { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblVirtualPurchase> tblVirtualPurchase { get; set; }
    }
}
