namespace Cores.DataService.Repository.IRepository;

public interface IUnitOfWork
{
    IApplicationUserRepository ApplicationUser{ get; }
    ICheckBoxRepository CheckBox { get; }
    IActivityLogRepository ActivityLog { get; }
    ITagRepository Tag { get; }
    IMessagePayloadRepository MessagePayload { get; }
    IContactRepository Contact { get; }
    ILanguageRepository Language { get; }
    IPurchaseRepository Purchase { get; }
    IPaymentMethodRepository PaymentMethod { get; }
    ICurrencyRepository Currency { get; }
    IProductRepository Product { get; }
    IOrderRepository Order { get; }
    IProblemRepository Problem { get; }
    IProblemTypeRepository ProblemType { get; }
    IEventRepository Event { get; }
    IEventTypeRepository EventType { get; }
    ITodoRepository Todo { get; }
    INotificationRepository Notification { get; }
    IDepartmentRepository Department { get; }
    IPositionRepository Position { get; }
    ILeaveTypeRepository LeaveType { get; }
    ILeaveRequestRepository LeaveRequest { get; }
    IAttendanceRepository Attendance { get; }
    IRecruitmentRepository Recruitment{ get; }
    IJobApplicationRepository JobApplication { get; }
    ISalaryRepository Salary { get; }
    IBenefitRepository Benefit { get; }
    IEmployeeBenefitRepository  EmployeeBenefit{ get; }
    IArchiveRepository  Archive { get; }
    IArchiveTypeRepository  ArchiveType { get; }
    ITrainingRepository Training { get; }
    IEmployeeTrainingRepository EmployeeTraining { get; }
    IPerformanceReviewRepository PerformanceReview{ get; }
    IVendorRepository Vendor{ get; } 
    ITransactionRepository Transaction { get; }
    IAccountRepository Account { get; }
    IJournalRepository Journal { get; }
    IJournalEntryRepository JournalEntry { get; }
    ITaxRepository Tax { get; }
    IDeductionRepository Deduction { get; }
    IEmployeeDeductionRepository EmployeeDeduction { get; }
    IPayrollRepository Payroll { get; }
    IWorkScheduleRepository WorkSchedule { get; }
    IEmployeeLeaveBalanceRepository EmployeeLeaveBalance { get; }
    IUnpaidLeaveDeductionRepository UnpaidLeaveDeduction { get; }
    IHolidayTypeRepository HolidayType { get; }
    IHolidayRepository Holiday { get; }
    ITransactionDetailRepository TransactionDetail { get; }
    IGeneralLedgerRepository GeneralLedger { get; }
    IDocumentRequestRepository DocumentRequest { get; }
    ICompensationRequestRepository CompensationRequest { get; }
    Task SaveAsync();
}