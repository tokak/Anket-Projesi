using System.Linq.Expressions;

namespace SurveyProject.DataAccess.Abstract
{
    /// <summary>
    /// Veri Erişim Katmanı (Data Access Layer) için genel arayüz.
    /// Temel veritabanı işlemlerini (CRUD) tanımlar.
    /// </summary>
    /// <typeparam name="T">İşlem yapılacak entity (varlık) türü.</typeparam>
    public interface IGenericDal<T> where T : class
    {
        /// <summary>
        /// Yeni bir varlık ekler.
        /// </summary>
        /// <param name="entity">Eklenecek varlık.</param>
        void Insert(T entity);

        /// <summary>
        /// Bir varlığı günceller.
        /// </summary>
        /// <param name="entity">Güncellenecek varlık.</param>
        void Update(T entity);

        /// <summary>
        /// Bir varlığı siler.
        /// </summary>
        /// <param name="entity">Silinecek varlık.</param>
        void Delete(T entity);

        /// <summary>
        /// Tüm varlıkları listeler.
        /// </summary>
        /// <returns>Tüm varlıkların listesi.</returns>
        List<T> GetListAll();

        /// <summary>
        /// Belirtilen ID'ye sahip varlığı getirir.
        /// </summary>
        /// <param name="id">Aranan varlığın ID'si.</param>
        /// <returns>Bulunan varlık.</returns>
        T GetById(int id);

        /// <summary>
        /// Belirli bir koşula uyan varlıkları listeler.
        /// </summary>
        /// <param name="filter">Filtreleme koşulu (LINQ Expression).</param>
        /// <returns>Filtrelenmiş varlıkların listesi.</returns>
        List<T> GetListAll(Expression<Func<T, bool>> filter);
    }
}
