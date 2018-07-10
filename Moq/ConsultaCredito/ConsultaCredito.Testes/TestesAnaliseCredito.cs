using System;
using System.Collections.Generic;
using Xunit;
using Moq;

namespace ConsultaCredito.Testes
{
    public class TestesAnaliseCredito
    {
        private Mock<IServicoConsultaCredito> mock;

        private const string CPF_INVALIDO = "123A";
        private const string CPF_ERRO_COMUNICACAO = "76217486300";
        private const string CPF_SEM_PENDENCIAS = "60487583752";
        private const string CPF_INADIMPLENTE = "82226651209";

        public TestesAnaliseCredito()
        {
            mock = new Mock<IServicoConsultaCredito>(MockBehavior.Strict);

            mock.Setup(s => s.ConsultarPendenciasPorCPF(CPF_INVALIDO))
                .Returns(() => null);

            mock.Setup(s => s.ConsultarPendenciasPorCPF(CPF_ERRO_COMUNICACAO))
                .Throws(new Exception("Testando erro de comunicação"));

            mock.Setup(s => s.ConsultarPendenciasPorCPF(CPF_SEM_PENDENCIAS))
                .Returns(() => new List<Pendencia>());

            Pendencia pendencia = new Pendencia();
            pendencia.CPF = CPF_INADIMPLENTE;
            pendencia.NomePessoa = "Cliente Teste";
            pendencia.NomeReclamante = "Empresas ACME";
            pendencia.DescricaoPendencia = "Parcela não paga";
            pendencia.VlPendencia = 900.50;
            List<Pendencia> pendencias = new List<Pendencia>();
            pendencias.Add(pendencia);

            mock.Setup(s => s.ConsultarPendenciasPorCPF(CPF_INADIMPLENTE))
                .Returns(() => pendencias);
        }

        private StatusConsultaCredito ObterStatusAnaliseCredito(
            string cpf)
        {
            AnaliseCredito analise = new AnaliseCredito(mock.Object);
            return analise.ConsultarSituacaoCPF(cpf);
        }

        [Fact]
        public void TesteCPFInvalido()
        {
            StatusConsultaCredito status =
                ObterStatusAnaliseCredito(CPF_INVALIDO);
            Assert.Equal(
                StatusConsultaCredito.ParametroEnvioInvalido, status);
        }

        [Fact]
        public void TesteErroComunicacao()
        {
            StatusConsultaCredito status =
                ObterStatusAnaliseCredito(CPF_ERRO_COMUNICACAO);
            Assert.Equal(
                StatusConsultaCredito.ErroComunicacao, status);
        }

        [Fact]
        public void TesteCPFSemPendencias()
        {
            StatusConsultaCredito status =
                ObterStatusAnaliseCredito(CPF_SEM_PENDENCIAS);
            Assert.Equal(
                StatusConsultaCredito.SemPendencias, status);
        }

        [Fact]
        public void TesteCPFInadimplente()
        {
            StatusConsultaCredito status =
                ObterStatusAnaliseCredito(CPF_INADIMPLENTE);
            Assert.Equal(
                StatusConsultaCredito.Inadimplente, status);
        }
    }
}