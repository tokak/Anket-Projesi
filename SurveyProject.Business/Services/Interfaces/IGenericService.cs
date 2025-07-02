namespace SurveyProject.Business.Services.Interfaces
{
    using System.Linq.Expressions;

    /// <summary>
    /// İş Katmanı (Business Layer) için genel servis arayüzü.
    /// Uygulamanın iş mantığı operasyonlarını tanımlar.
    /// </summary>
    /// <typeparam name="T">İşlem yapılacak entity (varlık) türü.</typeparam>
    public interface IGenericService<T> where T : class
    {
        /// <summary>
        /// Yeni bir varlık ekler. (İş kuralları burada uygulanabilir)
        /// </summary>
        /// <param name="entity">Eklenecek varlık.</param>
        void TAdd(T entity);

        /// <summary>
        /// Bir varlığı günceller.
        /// </summary>
        /// <param name="entity">Güncellenecek varlık.</param>
        void TUpdate(T entity);

        /// <summary>
        /// Bir varlığı siler.
        /// </summary>
        /// <param name="entity">Silinecek varlık.</param>
        void TDelete(T entity);

        /// <summary>
        /// Tüm varlıkları listeler.
        /// </summary>
        /// <returns>Tüm varlıkların listesi.</returns>
        List<T> TGetList();

        /// <summary>
        /// Belirtilen ID'ye sahip varlığı getirir.
        /// </summary>
        /// <param name="id">Aranan varlığın ID'si.</param>
        /// <returns>Bulunan varlık.</returns>
        T TGetById(int id);

        /// <summary>
        /// Belirli bir koşula uyan varlıkları listeler.
        /// </summary>
        /// <param name="filter">Filtreleme koşulu.</param>
        /// <returns>Filtrelenmiş varlıkların listesi.</returns>
        List<T> TGetListByFilter(Expression<Func<T, bool>> filter);
    }
}
