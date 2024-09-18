using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
namespace FormsApp.Models{

    public class Product{
       
       [Display(Name="Urun Id")]
      // [BindNever] //böylece ürün ıd form üzerinden bind edilmez.
       public int ProductId { get; set; }

       [Display(Name="Urun Adı")]
       [Required(ErrorMessage ="gerekli bir alan")] //required gerekliliktir. bir value'nin set edilmesini zorunlu hale getirmiş oluyoruz. Ayrıca errorMessage ile de türkçeleştirmiş olduk.
       [StringLength(100)]//maksimum  karakter validasyonu
       public string Name { get; set; } = null!; //String nullable olmaması için uyarı verir. null olamaz.

       [Display(Name="Fiyat")]
       [Required]
       [Range(0,100000)]//bu da bir validasyondur fiyatı min 0 max 100.000 şekilde post edebilmesini sağlamış olduk.
       public decimal? Price { get; set; } //? null değer alınabilir yaptı. Bunu required'ın işe yaraması için yapıyoruz. Böylece boş geçilemez oluyor.
       [Display(Name="Resim")]
       [Required]
       public string? Image { get; set; } = string.Empty; //empty olabilir sorun yok demiş olduk.
       public bool IsActive { get; set; }
       [Display(Name="Category")]
       [Required]
       public int? CategoryId { get; set; }//her bir ürünün hangi kategoriye ait olduğunu anlamak için.

         





    }
}