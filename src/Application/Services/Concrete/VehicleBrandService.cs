using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Application.Infrastructure.Persistence;
using Domain.DTOs;
using Application.Services.Common;

namespace Application.Services.Concrete
{
    public class VehicleBrandService : BaseService,IVehicleBrandService
    {
      

        public VehicleBrandService(ICarRentalDbContext context) : base(context)
        {
            
        }
        
        public Response Add(VehicleBrand vehicleBrand)
        {
            var CheckResponse = CheckToAddOrUpdate(vehicleBrand);
            if (!CheckResponse.IsSuccess)
                return CheckResponse;

            Context.VehicleBrand.Add(vehicleBrand);
            Context.SaveChanges();

            return Response.Success("Marka başarıyla kaydedildi");
        }
        private Response CheckToAddOrUpdate(VehicleBrand vehicleBrand)
        {
            bool isUpdate = vehicleBrand.Id > 0;
            
            int sameNumberOfRecords = (from b in Context.VehicleBrand
                                       where b.Name == vehicleBrand.Name && b.Id != vehicleBrand.Id
                                       select b
                                       ).Count();
            if(sameNumberOfRecords > 0)
            {
                return Response.Fail($"{vehicleBrand.Name} markası sistemde zaten kayıtlıdır.");
            }
            if(isUpdate)
            {
                int numberOfModels = Context.VehicleModel.Where(m => m.VehicleBrandId == vehicleBrand.Id).Count();
                if(numberOfModels > 0)
                {
                    return Response.Fail($"{vehicleBrand.Name} markasına ait {numberOfModels} adet model olduğu için bu kayıt silinemez.");
                }
            }
            
            
            
            
            return Response.Success();
        }
        
        public Response Update(VehicleBrand vehicleBrand)
        {
            var checkResponse = CheckToAddOrUpdate(vehicleBrand);
            if (!checkResponse.IsSuccess)
                return checkResponse;
            
            var vehicleBrandToUpdate = GetById(vehicleBrand.Id);
            vehicleBrandToUpdate.Name = vehicleBrand.Name;
            Context.SaveChanges();

            return Response.Success("Marka başarıyla güncellendi");
        }
        public Response Delete(int id)
        {
            var vehicleBrandToDelete = GetById(id);

            var checkResponse = CheckToDelete(vehicleBrandToDelete);
            if (!checkResponse.IsSuccess)
                return checkResponse;
            
            Context.VehicleBrand.Remove(vehicleBrandToDelete);
            Context.SaveChanges();

            return Response.Success("Marka başarıyla silindi");
        }
        private Response CheckToDelete(VehicleBrand vehicleBrand)
        {
            #region check Related Models
            int numberOfModels = Context.VehicleModel.Where(m => m.VehicleBrandId == vehicleBrand.Id).Count();
            if (numberOfModels > 0)
            {
                return Response.Fail($"{vehicleBrand.Name} markasına ait {numberOfModels} adet model olduğu için bu kayıt silinemez.");
            }
            #endregion
            return Response.Success();
        }

        public VehicleBrand GetById(int id)
        {
            return Context.VehicleBrand.Where(v => v.Id == id).SingleOrDefault();
        }

        public List<VehicleBrand> Get(VehicleBrandFilter filter)
        {
            var items = (from v in Context.VehicleBrand
                         where v.Name.StartsWith(filter.Name)
                         orderby v.Name
                         select v).ToList();
            return items;
        }

        public string GetName()
        {
            return "Vehicle brand service";
        }
    }
      
        
    
}
