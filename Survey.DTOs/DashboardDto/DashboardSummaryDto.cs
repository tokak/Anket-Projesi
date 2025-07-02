namespace Survey.DTOs.DashboardDto
{
    public class DashboardSummaryDto
    {
        //Tamamlanan Anket Metrikleri  
        public int TotalCompletedSurveys { get; set; } // Tamamlanmış anket kayıtlarının sayısı (bir anketin birden çok kez tamamlanması dahil)
        public int TotalUniqueCompletedSurveys { get; set; } // Kaç farklı anketin en az bir kez tamamlandığı
        public int TotalQuestionsInCompletedSurveys { get; set; } // Tamamlanan anketlerdeki toplam soru sayısı
        public int TotalAnswersInCompletedSurveys { get; set; } // Tamamlanan anketlerdeki toplam cevap sayısı

        //Tamamlanmayan Anket Metrikleri  
        public int TotalInProgressSurveys { get; set; } // Devam eden (tamamlanmamış) anket kayıtlarının sayısı
        public int TotalUniqueInProgressSurveys { get; set; } // Kaç farklı anketin devam etmekte olduğu
        public int TotalQuestionsInInProgressSurveys { get; set; } // Devam eden anketlerdeki toplam soru sayısı
        public int TotalAnswersInInProgressSurveys { get; set; } // Devam eden anketlerdeki toplam cevap sayısı
    }
}