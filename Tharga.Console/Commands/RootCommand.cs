using System;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Tharga.Console.Commands.Base;
using Tharga.Console.Interfaces;
using Tharga.Runtime;

namespace Tharga.Console.Commands;

public sealed class RootCommandIoc : RootCommandBase
{
	private readonly ServiceCollection _serviceCollection;
	private ServiceProvider _serviceProvider;

	public RootCommandIoc(IConsole console)
		: base(console)
	{
		_serviceCollection = new ServiceCollection();
		_ = AssemblyService.GetTypes<ICommand>().Select(_serviceCollection.AddTransient).ToArray();
	}

	public IServiceCollection ServiceCollection => _serviceProvider == null ? _serviceCollection : throw new InvalidOperationException("The service provider has already been built. It is built when the engine is created.");
	public IServiceProvider ServiceProvider => _serviceProvider ?? throw new InvalidOperationException("The service provider has not been built yet. It is built when the engine is created.");

	protected override ICommandResolver BuildCommandResolver()
	{
		if (_serviceProvider != null) throw new InvalidOperationException("The command resolver has already been built.");

		_serviceProvider = _serviceCollection.BuildServiceProvider();
		return new CommandResolver(type => (ICommand)_serviceProvider.GetService(type));
	}
}