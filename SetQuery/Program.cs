using System;
using System.IO;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace SetQuery
{
    class Program
    {
        static void Main(string[] args)
        {
            //処理後画像のディレクトリパス
            var rec_path = "C:\\Users\\melo3611\\Desktop\\test_rec";
            DirectoryInfo rec_di = new DirectoryInfo(rec_path);
            FileInfo[] rec_fiAlls = rec_di.GetFiles();

            //原画像のディレクトリパス
            var send_path = "C:\\Users\\melo3611\\Desktop\\test_send";
            DirectoryInfo send_di = new DirectoryInfo(send_path);
            FileInfo[] send_fiAlls = send_di.GetFiles();

            int rec_Files = Directory.GetFiles(rec_path, "*", SearchOption.TopDirectoryOnly).Length;
            int send_Files = Directory.GetFiles(send_path, "*", SearchOption.TopDirectoryOnly).Length;

            if(rec_Files == send_Files)
            {
                for(int loop = 0; loop < rec_Files; loop++)
                {
                    var imgr_path = rec_fiAlls[loop].FullName;

                    Debug.WriteLine(" get " + imgr_path);

                    var imgs_path = send_fiAlls[loop].FullName;

                    Debug.WriteLine(" get " + imgs_path);

                    // 原画像ファイル/Metadata読み込み
                    Uri uri = new Uri(imgs_path, UriKind.Absolute);
                    BitmapFrame send_frame = BitmapFrame.Create(uri);
                    BitmapMetadata send_metadata = send_frame.Metadata as BitmapMetadata;

                    //Metaデータの取り出し
                    var ResolutionUnit = send_metadata.GetQuery("/app1/ifd/{ushort=296}");      //解像度の単位
                    var Make = send_metadata.GetQuery("/app1/ifd/{ushort=271}");                    // メーカー名
                    var Model = send_metadata.GetQuery("/app1/ifd/{ushort=272}");                    // モデル名

                    var ExifVer = send_metadata.GetQuery("/app1/ifd/exif/{ushort=36864}");         // Exifバージョン
                    var ExposureTime = send_metadata.GetQuery("/app1/ifd/exif/{ushort=33434}");     //露出時間
                    var ApertureValue = send_metadata.GetQuery("/app1/ifd/exif/{ushort=37378}");        //絞り値
                                                                                                        //var ISOspeed = send_metadata.GetQuery("/app1/ifd/exif/{ushort=34867}");     //ISO速度
                                                                                                        //露出補正、焦点距離、最大絞り


                    var GPSLatitudeRef = send_metadata.GetQuery("/app1/ifd/gps/subifd:{ushort=1}");  // 北緯or南緯
                    var GPSLatitude = send_metadata.GetQuery("/app1/ifd/gps/subifd:{ushort=2}");     // 緯度
                    var GPSLongitudeRef = send_metadata.GetQuery("/app1/ifd/gps/{ushort=3}");        // 東経or西経
                    var GPSLongitude = send_metadata.GetQuery("/app1/ifd/gps/{ushort=4}");           // 経度
                    var GPSAltitudeRef = send_metadata.GetQuery("/app1/ifd/gps/{ushort=5}");        //高度基準
                    var GPSAltitude = send_metadata.GetQuery("/app1/ifd/gps/{ushort=6}");       //高度


                    //処理後ファイル読み込み
                    MemoryStream data = new MemoryStream(File.ReadAllBytes(imgr_path));
                    WriteableBitmap image = new WriteableBitmap(BitmapFrame.Create(data));
                    data.Close();

                    //metaデータ準備
                    var metadata = new BitmapMetadata("jpg");

                    metadata.SetQuery("/app1/ifd/{ushort=296}", ResolutionUnit);
                    metadata.SetQuery("/app1/ifd/{ushort=271}", Make);
                    metadata.SetQuery("/app1/ifd/{ushort=272}", Model);

                    metadata.SetQuery("/app1/ifd/exif/{ushort=36864}", ExifVer);
                    metadata.SetQuery("/app1/ifd/exif/{ushort=33434}", ExposureTime);
                    metadata.SetQuery("/app1/ifd/exif/{ushort=37378}", ApertureValue);
                    //metadata.SetQuery("/app1/ifd/exif/{ushort=34867}", ISOspeed);

                    metadata.SetQuery("/app1/ifd/gps/subifd:{ushort=1}", GPSLatitudeRef);
                    metadata.SetQuery("/app1/ifd/gps/subifd:{ushort=2}", GPSLatitude);
                    metadata.SetQuery("/app1/ifd/gps/subifd:{ushort=3}", GPSLongitudeRef);
                    metadata.SetQuery("/app1/ifd/gps/subifd:{ushort=4}", GPSLongitude);
                    metadata.SetQuery("/app1/ifd/gps/subifd:{ushort=5}", GPSAltitudeRef);
                    metadata.SetQuery("/app1/ifd/gps/subifd:{ushort=6}", GPSAltitude);

                    //ファイル書き込み
                    using (FileStream stream = new FileStream(imgr_path, FileMode.Open))
                    {
                        var enc = new JpegBitmapEncoder();
                        var frame = BitmapFrame.Create(image, null, metadata, null);
                        enc.Frames.Add(frame);
                        enc.Save(stream);
                    }
                }
            }

            else
            {
                Debug.WriteLine('send and receive imagefiles are mismatching')
            }

        }
    }
}
