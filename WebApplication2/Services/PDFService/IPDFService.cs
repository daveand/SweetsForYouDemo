using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Services.PDFService
{
    public interface IPDFService
    {
        Task<byte[]> Create(OrdersPdfModel orders);
    }
}