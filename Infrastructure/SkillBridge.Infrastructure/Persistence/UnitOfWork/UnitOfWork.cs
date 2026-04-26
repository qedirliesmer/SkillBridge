using SkillBridge.Application.Interfaces;
using SkillBridge.Application.UnitOfWork;
using SkillBridge.Infrastructure.Persistence.Context;
using SkillBridge.Infrastructure.Persistence.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Infrastructure.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly SkillBridgeDbContext _context;
    private IMentorProfileRepository? _mentorProfiles;
    private IUserProfileRepository? _userProfiles;
    private IAvailabilityRepository? _availabilities;
    private IBookingRepository? _bookings;
    private Hashtable? _repositories;


    public UnitOfWork(SkillBridgeDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IMentorProfileRepository MentorProfiles =>
        _mentorProfiles ??= new MentorProfileRepository(_context);
    public IUserProfileRepository UserProfiles =>
        _userProfiles ??= new UserProfileRepository(_context);

    public IAvailabilityRepository Availabilities =>
        _availabilities ??= new AvailabilityRepository(_context);
    public IBookingRepository Bookings =>
        _bookings ??= new BookingRepository(_context);
    public IGenericRepository<T> Repository<T>() where T : class
    {
        if (_repositories == null) _repositories = new Hashtable();

        var type = typeof(T).Name;

        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(GenericRepository<>);
            var repositoryInstance = Activator.CreateInstance(
                repositoryType.MakeGenericType(typeof(T)), _context);

            _repositories.Add(type, repositoryInstance);
        }

        return (IGenericRepository<T>)_repositories[type]!;
    }
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}