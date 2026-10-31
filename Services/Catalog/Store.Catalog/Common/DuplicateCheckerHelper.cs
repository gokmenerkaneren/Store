namespace Store.Catalog.Common
{
    public static class DuplicateCheckerHelper
    {
        public static string BuildDuplicateMessage(string fieldLabel, string value, bool isDeleted, bool isActive)
        {
            string statusHtml;
            if (isDeleted)
            {
                statusHtml = "<strong style=\"color: #000000; font-weight: bold;\">SİLİNMİŞ</strong>";
            }
            else if (isActive)
            {
                statusHtml = "<strong style=\"color: #2ed8b6; font-style: italic;\"><em>AKTİF</em></strong>";
            }
            else
            {
                statusHtml = "<strong style=\"color: #ffb64d; font-style: italic;\"><em>PASİF</em></strong>";
            }

            string suggestionText = (isDeleted || !isActive)
                ? "Bu kaydı AKTİF hale getirmenizi öneririz!"
                : "Verilerinizi kontrol edip tekrar denemenizi rica ederiz!";

            string upperValue = (value ?? "").ToUpper();

            return $"Kaydetmeye çalıştığınız '<strong>{fieldLabel} : {upperValue}</strong>' olarak {statusHtml} bir kayıt var.<br/>{suggestionText}";
        }
    }
}

