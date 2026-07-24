using MBA.Core.Messages;

namespace MBA.Core.Testes;

// Dublês concretos das classes-base abstratas/protegidas de MBA.Core.Messages,
// usados para exercitar o MediatorHandler e o comportamento das mensagens.
public class FakeCommand : Command { }

public class FakeCommandRaiz : CommandRaiz { }

public class FakeEvent : Event { }
