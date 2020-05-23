using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.Entities
{
    [Table("Notes")]
    public class Note : MyEntityBase
    {
        [DisplayName("Başlık"), Required(ErrorMessage = "{0} alanı gereklidir."), StringLength(60, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Title { get; set; }

        [DisplayName("Açıklama"), Required(ErrorMessage = "{0} alanı gereklidir."), StringLength(2000, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Text { get; set; }

        [DisplayName("Taslak mı?")]
        public bool IsDraft { get; set; }
        [DisplayName("Beğeni Sayısı")]
        public int LikeCount { get; set; }
        public int CategoriId { get; set; }

        [DisplayName("Kategori")]
        public virtual Category Categori { get; set; }
        public virtual EvernoteUser Owner { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public virtual List<Liked> Likes { get; set; }

        //Comment listesinin oto olarak oluşmasını sağlar.
        public Note()
        {
            Comments = new List<Comment>();
            Likes = new List<Liked>();
        }
    }
}
