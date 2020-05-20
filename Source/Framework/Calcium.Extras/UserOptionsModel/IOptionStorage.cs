using System.Threading.Tasks;

namespace Calcium.UserOptionsModel
{
	public interface IOptionStorage<TSetting>
	{
		bool CanGetSetting { get; }
		Task<TSetting> GetSetting();

		bool CanSaveSetting { get; }
		Task<SaveOptionResult> SaveSetting(TSetting setting);
	}
}
