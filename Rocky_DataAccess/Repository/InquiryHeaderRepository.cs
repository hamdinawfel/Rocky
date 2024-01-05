using Rocky_DataAccess.Data;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;

namespace Rocky_DataAccess.Repository
{
    public class InquiryHeaderRopository : Repository<InquiryHeader>, IInquiryHeaderRepository
    {
        private readonly RockyDbContext _db;
        public InquiryHeaderRopository(RockyDbContext db) : base(db)
        {
            _db = db;   
        }

        public void Update(InquiryHeader obj)
        {
            _db.InquiryHeader.Update(obj);
        }
    }
}
