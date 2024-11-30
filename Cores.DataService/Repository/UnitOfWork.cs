using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;

namespace Cores.DataService.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _db;
    public IApplicationUserRepository ApplicationUser { get; }
    public ICheckBoxRepository CheckBox { get; }
    public IActivityLogRepository ActivityLog { get; }
    public ITagRepository Tag { get; }
    public IMessagePayloadRepository MessagePayload { get; }
    public IContactRepository Contact { get; }
    public ILanguageRepository Language { get; }
    public IPurchaseRepository Purchase { get; }
    public IPaymentMethodRepository PaymentMethod { get; }
    public ICurrencyRepository Currency { get; }
    public IProductRepository Product { get; }
    public IOrderRepository Order { get; }
    public IProblemRepository Problem { get; }
    public IProblemTypeRepository ProblemType { get; }
    public IEventRepository Event { get; }
    public IEventTypeRepository EventType { get; }
    public ITodoRepository Todo { get; }
    public INotificationRepository Notification { get; }
    public IDepartmentRepository Department { get; }
    public IPositionRepository Position { get; }
    public ILeaveTypeRepository LeaveType { get; }
    public ILeaveRequestRepository LeaveRequest { get; }
    public IAttendanceRepository Attendance { get; }
    public IRecruitmentRepository Recruitment { get; }
    public IJobApplicationRepository JobApplication { get; }
    public ISalaryRepository Salary { get; }
    public IBenefitRepository Benefit { get; }
    public IEmployeeBenefitRepository EmployeeBenefit { get; }
    public IArchiveRepository Archive { get; }
    public IArchiveTypeRepository ArchiveType { get; }
    public ITrainingRepository Training { get; }
    public IEmployeeTrainingRepository EmployeeTraining { get; }
    public IPerformanceReviewRepository PerformanceReview { get; }
    public IVendorRepository Vendor { get; }
    public ITransactionRepository Transaction { get; }
    public IAccountRepository Account { get; }
    public IJournalRepository Journal { get; }
    public IJournalEntryRepository JournalEntry { get; }
    public ITaxRepository Tax { get; }
    public IDeductionRepository Deduction { get; }
    public IEmployeeDeductionRepository EmployeeDeduction { get; }
    public IPayrollRepository Payroll { get; }
    public IWorkScheduleRepository WorkSchedule { get; }
    public IEmployeeLeaveBalanceRepository EmployeeLeaveBalance { get; }
    public IUnpaidLeaveDeductionRepository UnpaidLeaveDeduction { get; }
    public IHolidayTypeRepository HolidayType { get; }
    public IHolidayRepository Holiday { get; }
    public ITransactionDetailRepository TransactionDetail { get; }
    public IGeneralLedgerRepository GeneralLedger { get; }

    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;
        ApplicationUser = new ApplicationUserRepository(_db);
        CheckBox = new CheckBoxRepository(_db);
        ActivityLog = new ActivityLogRepository(_db);
        Tag = new TagRepository(_db);
        MessagePayload = new MessagePayloadRepository(_db);
        Contact = new ContactRepository(_db);
        Language = new LanguageRepository(_db);
        Purchase = new PurchaseRepository(_db);
        PaymentMethod = new PaymentMethodRepository(_db);
        Currency = new CurrencyRepository(_db);
        Product = new ProductRepository(_db);
        Order = new OrderRepository(_db);
        Problem = new ProblemRepository(_db);
        ProblemType = new ProblemTypeRepository(_db);
        Event = new EventRepository(_db);
        EventType = new EventTypeRepository(_db);
        Todo = new TodoRepository(_db);
        Notification = new NotificationRepository(_db);
        Department = new DepartmentRepository(_db);
        Position = new PositionRepository(_db);
        LeaveType = new LeaveTypeRepository(_db);
        LeaveRequest = new LeaveRequestRepository(_db);
        Attendance = new AttendanceRepository(_db);
        Recruitment = new RecruitmentRepository(_db);
        JobApplication = new JobApplicationRepository(_db);
        Salary = new SalaryRepository(_db);
        Benefit = new BenefitRepository(_db);
        EmployeeBenefit = new EmployeeBenefitRepository(_db);
        Archive = new ArchiveRepository(_db);
        ArchiveType = new ArchiveTypeRepository(_db);
        Training = new TrainingRepository(_db);
        EmployeeTraining = new EmployeeTrainingRepository(_db);
        PerformanceReview = new PerformanceReviewRepository(_db);
        Vendor = new VendorRepository(_db);
        Transaction = new TransactionRepository(_db);
        Account = new AccountRepository(_db);
        Journal = new JournalRepository(_db);
        JournalEntry = new JournalEntryRepository(_db);
        Tax = new TaxRepository(_db);
        Deduction = new DeductionRepository(_db);
        EmployeeDeduction = new EmployeeDeductionRepository(_db);
        Payroll = new PayrollRepository(_db);
        WorkSchedule = new WorkScheduleRepository(_db);
        EmployeeLeaveBalance = new EmployeeLeaveBalanceRepository(_db);
        UnpaidLeaveDeduction = new UnpaidLeaveDeductionRepository(_db);
        HolidayType = new HolidayTypeRepository(_db);
        Holiday = new HolidayRepository(_db);
        TransactionDetail = new TransactionDetailRepository(_db);
        GeneralLedger = new GeneralLedgerRepository(_db);
    }

    public async Task SaveAsync() => await _db.SaveChangesAsync();
}