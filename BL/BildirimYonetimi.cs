using System;
using System.Collections.Generic;
using System.Data;
using CafeApp.DAL;
using CafeApp.Models;

namespace CafeApp.BL
{
    public class BildirimYonetimi
    {
        private CafeVeriErisim _dal = new CafeVeriErisim();

        public List<HazirBildirim> HazirBekleyenleriGetir()
        {
            var liste = new List<HazirBildirim>();
            DataTable dt = _dal.HazirBekleyenler();

            foreach (DataRow row in dt.Rows)
            {
                liste.Add(new HazirBildirim
                {
                    SiparisDetayId = Convert.ToInt32(row["siparis_detay_id"]),
                    SiparisId      = Convert.ToInt32(row["siparis_id"]),
                    Adet           = Convert.ToInt32(row["adet"]),
                    UrunAdi        = row["urun_adi"]?.ToString() ?? "",
                    MasaNo         = row["masa_no"]?.ToString() ?? ""
                });
            }
            return liste;
        }

        public void GorulduIsaretle(int siparisDetayId)
        {
            _dal.GarsonGorduIsaretle(siparisDetayId);
        }
    }
}
