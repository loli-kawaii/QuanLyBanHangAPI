﻿using System.Linq;
using QuanLyBanHangAPI.Models.NhaCungCap;
using System.Collections.Generic;
using QuanLyBanHangAPI.Data;

namespace QuanLyBanHangAPI.Services.NhaCungCapServices
{
    public class NhaCungCapServices : INhaCungCapServices
    {
        private readonly DB _db;
        public NhaCungCapServices(DB db)
        {
            _db = db;
        }
        public NhaCungCapVM Add(NhaCungCapModel model)
        {
            var ncc = new NhaCungCap
            {
                TenNhaCungCap = model.TenNhaCungCap,
                TrangChu = model.TrangChu
            };
            _db.Add(ncc);
            _db.SaveChanges();
            return new NhaCungCapVM
            {
                MaNhaCungCap = ncc.MaNhaCungCap,
                TenNhaCungCap = ncc.TenNhaCungCap,
                TrangChu = ncc.TrangChu
            };
        }

        public void DeleteById(int id)
        {
            var ncc = _db.NhaCungCaps.SingleOrDefault(n => n.MaNhaCungCap == id);
            if(ncc != null)
            {
                _db.Remove(ncc);
                _db.SaveChanges();
            }
        }

        public List<NhaCungCapVM> GetAll()
        {
            var nccs = _db.NhaCungCaps.Select(n => new NhaCungCapVM
            {
                MaNhaCungCap = n.MaNhaCungCap,
                TenNhaCungCap = n.TenNhaCungCap,
                TrangChu = n.TrangChu
            });
            return nccs.ToList();
        }

        public NhaCungCapVM GetById(int id)
        {
            var ncc = _db.NhaCungCaps.SingleOrDefault(n => n.MaNhaCungCap == id);
            if (ncc != null)
            {
                return new NhaCungCapVM
                {
                    MaNhaCungCap = ncc.MaNhaCungCap,
                    TenNhaCungCap = ncc.TenNhaCungCap,
                    TrangChu = ncc.TrangChu
                };
            }
            return null;
        }

        public void Update(NhaCungCapVM vm)
        {
            var ncc = _db.NhaCungCaps.SingleOrDefault(n => n.MaNhaCungCap == vm.MaNhaCungCap);
            if (ncc != null)
            {
                ncc.TenNhaCungCap = vm.TenNhaCungCap;
                ncc.TrangChu = vm.TrangChu;
                _db.SaveChanges();
            }
        }
    }
}
