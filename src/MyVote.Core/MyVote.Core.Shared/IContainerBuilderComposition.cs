using Autofac;

namespace MyVote.Core
{
	/// <summary>
	/// Defines a contract to building a container
	/// with all the required parts.
	/// </summary>
	/// <remarks>
	/// The intent of this interface is that a developer will
	/// define 1 or more builders in an assembly. The bootstrapper
	/// will automatically find all the classes that implement this
	/// interface, create instances of them, and call <seealso cref="Compose"/>
	/// to add their needed dependencies into the container.
	/// </remarks>
	public interface IContainerBuilderComposition
	{
		/// <summary>
		/// Registers needed components.
		/// </summary>
		/// <param name="builder">The builder to add registrations to.</param>
		void Compose(ContainerBuilder builder);
	}
}
