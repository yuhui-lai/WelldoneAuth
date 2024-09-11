using QRCoder;

namespace WelldoneAuth.Lib.Utilities
{
    public static class QrcodeUtil
    {
        public static byte[] Create(string content)
        {
            using QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
            // ECC Level 是 Error Correction Code Level 的縮寫，中文意思是「錯誤更正等級」，這個等級越高，可以容許的錯誤越多
            // L<M<Q<H
            using QRCodeData data = qRCodeGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            using PngByteQRCode qRCode = new PngByteQRCode(data);
            byte[] image = qRCode.GetGraphic(10);
            return image;
        }

        public static byte[] Create(string content, byte[] darkColor, byte[] lightColor)
        {
            using QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
            // ECC Level 是 Error Correction Code Level 的縮寫，中文意思是「錯誤更正等級」，這個等級越高，可以容許的錯誤越多
            // L<M<Q<H
            using QRCodeData data = qRCodeGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            using PngByteQRCode qRCode = new PngByteQRCode(data);
            // 變更 QRCode 圖片的顏色，內容則為 R, G, B 三原色的色碼
            byte[] image = qRCode.GetGraphic(10,
                                darkColorRgba: darkColor,
                                lightColorRgba: lightColor);
            return image;
        }
    }
}
