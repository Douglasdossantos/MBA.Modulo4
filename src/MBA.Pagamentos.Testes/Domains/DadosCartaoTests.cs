


using FluentAssertions;
using MBA.Core.DomainObjects;
using MBA.Pagamentos.Domain.ValueObjects;

namespace MBA.Pagamentos.Testes.Domains;
public class DadosCartaoTests
{
    #region Helpers
    private const string _numeroValido = "5493813493490144";
    private const string _nomeValido = "Jairo Azevedo";
    private const string _validadeValida = "12/96";
    private const string _cvvValido = "593";

    private static DadosCartao CriarCartao(string numero = _numeroValido,
                                           string nome = _nomeValido,
                                           string validade = _validadeValida,
                                           string cvv = _cvvValido)
    {
        return new DadosCartao(numero, nome, validade, cvv);
    }
    #endregion

    #region Construtores
    [Fact]
    public void Deve_criar_dados_cartao_validos()
    {
        var cartao = CriarCartao();

        cartao.Should().NotBeNull();
        cartao.Numero.Should().Be(_numeroValido);
        cartao.NomeTitular.Should().Be(_nomeValido);
        cartao.Validade.Should().Be(_validadeValida);
        cartao.CVV.Should().Be(_cvvValido);
    }

    [Theory]
    [InlineData(null, _nomeValido, _validadeValida, _cvvValido, "*Número do cartão deve ser informado*")]
    [InlineData("123", _nomeValido, _validadeValida, _cvvValido, "*Número do cartão deve possuir 16 caracteres*")]
    [InlineData("12345678901234567", _nomeValido, _validadeValida, _cvvValido, "*Número do cartão deve possuir 16 caracteres*")]
    [InlineData(_numeroValido, null, _validadeValida, _cvvValido, "*Nome do titular deve ser informado*")]
    [InlineData(_numeroValido, "ab", _validadeValida, _cvvValido, "*Nome do titular deve ter entre 3 e 50 caracteres*")]
    [InlineData(_numeroValido, _nomeValido, null, _cvvValido, "*Validade do cartão deve ser informada*")]
    [InlineData(_numeroValido, _nomeValido, "13/29", _cvvValido, "*Validade do cartão deve estar no formato MM/AA*")]
    [InlineData(_numeroValido, _nomeValido, _validadeValida, null, "*CVV deve ser informado*")]
    [InlineData(_numeroValido, _nomeValido, _validadeValida, "12", "*CVV deve possuir 3 caracteres*")]
    [InlineData(_numeroValido, _nomeValido, _validadeValida, "1283", "*CVV deve possuir 3 caracteres*")]
    [InlineData(_numeroValido, _nomeValido, _validadeValida, "abc", "*CVV deve conter apenas números*")]
    public void Nao_deve_criar_dados_cartao_invalidos(string numero, string nome, string validade, string cvv, string mensagemEsperada)
    {
        Action act = () => CriarCartao(numero ?? string.Empty,
            nome ?? string.Empty,
            validade ?? string.Empty,
            cvv ?? string.Empty);

        act.Should().Throw<DomainException>()
           .WithMessage(mensagemEsperada);
    }
    #endregion

    #region Metodos do Dominio
    #endregion

    #region Overrides
    [Fact]
    public void ToString_deve_conter_nome_e_validade()
    {
        var cartao = CriarCartao();
        cartao.ToString().Should().Contain(_nomeValido)
                              .And.Contain(_validadeValida);
    }
    #endregion
}
