using System.Reflection;

namespace DynamicApiServer.Execution.Executors.CSharpScript;

public class CSharpScriptReflector
{
	private readonly ILogger<CSharpScriptReflector> _logger;

	public CSharpScriptReflector(ILoggerFactory loggerFactory)

	{
		_logger = loggerFactory.CreateLogger<CSharpScriptReflector>();

	}

	public MethodInfo GetMethodFromType(Type type)
	{
		var method = type.GetMethod("ExecuteAsync");
		return method;
	}

	public Type GetTypeFromAssembly(Assembly assembly)
	{
		_logger.LogInformation("Finding script class...");
		var type = assembly.GetTypes().FirstOrDefault(t => t.GetInterfaces().Contains(typeof(IDynamicEndpointExecutor)));
		return type;
	}

	public object GetClassInstanceFromType(Type type)
	{
		if (type == null)
		{
			_logger.LogError("No class found that implements IDynamicEndpointExecutor.");
			return "Error: No class found that implements IDynamicEndpointExecutor.";
		}
		_logger.LogInformation("Script class found: {0}", type.Name);

		_logger.LogInformation("Instantiating script class...");
		var instance = Activator.CreateInstance(type);

		return instance;
	}

	public Assembly GetAssemblyFromMemoryStream(MemoryStream ms)
	{
		ms.Seek(0, SeekOrigin.Begin);
		var assembly = Assembly.Load(ms.ToArray());

		_logger.LogInformation("Listing all types in the assembly...");
		foreach (var t in assembly.GetTypes())
		{
			_logger.LogInformation("Found type: {0}", t.FullName);
		}

		return assembly;
	}

}