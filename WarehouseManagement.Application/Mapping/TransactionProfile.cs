
using AutoMapper;
using WarehouseManagement.Application.DTOs.WarehouseTransactions;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Application.Mapping
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<WarehouseTransaction, WarehouseTransactionDto>();
        }
    }
}
