using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaliSahaOtomasyon.Model
{
    public class Randevu
    {
        public int Randevuid { get; set; }
        public string uyead { get; set; }
        public string randevutarih{get; set;}
        public string randevusaat { get; set;}
        public string sahadurum { get; set; }
        public int ucret { get; set; }

        //public override string ToString()
        //{
        //    return $"Randevu ID: {Randevuid}\nUye Ad: {uyead}\nRandevu Tarih: {randevutarih}\nRandevu Saat: {randevusaat}\nSaha Durum: {randevusaat}\nUcret: {ucret}";
        //}
    }
}
