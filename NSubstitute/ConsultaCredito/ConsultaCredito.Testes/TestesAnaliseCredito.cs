using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace ConsultaCredito.Testes
{
    [TestClass]
    public class TestesAnaliseCredito
    {
        private const string CPF_INVALIDO = "123A";
        private const string CPF_ERRO_COMUNICACAO = "76217486300";
        private const string CPF_SEM_PENDENCIAS = "60487583752";
        private const string CPF_INADIMPLENTE = "82226651209";

        private IServicoConsultaCredito mock;

        public TestesAnaliseCredito()
        {
            mock = Substitute.For<IServicoConsultaCredito>();

            mock.ConsultarPendenciasPorCPF(CPF_INVALIDO)
                .Returns((List<Pendencia>)null);

            mock.ConsultarPendenciasPorCPF(CPF_ERRO_COMUNICACAO)
                .Returns(s => { throw new Exception("Erro de comunicação..."); });

            mock.ConsultarPendenciasPorCPF(CPF_SEM_PENDENCIAS)
                .Returns(new List<Pendencia>());

            Pendencia pendencia = new Pendencia();
            pendencia.CPF = CPF_INADIMPLENTE;
            pendencia.NomePessoa = "João da Silva";
            pendencia.NomeReclamante = "Empresa XYZ";
            pendencia.DescricaoPendencia = "Parcela não paga";
            pendencia.VlPendencia = 700;
            List<Pendencia> pendencias = new List<Pendencia>();
            pendencias.Add(pendencia);

            mock.ConsultarPendenciasPorCPF(CPF_INADIMPLENTE)
                .Returns(pendencias);
        }

        private StatusConsultaCredito ObterStatusAnaliseCredito(string cpf)
        {
            AnaliseCredito analise = new AnaliseCredito(mock);
            return analise.ConsultarSituacaoCPF(cpf);
        }

        [TestMethod]
        public void TesteCPFInvalido()
        {
            StatusConsultaCredito status =
                ObterStatusAnaliseCredito(CPF_INVALIDO);
            Assert.AreEqual(
                StatusConsultaCredito.ParametroEnvioInvalido, status);
        }

        [TestMethod]
        public void TesteErroComunicacao()
        {
            StatusConsultaCredito status =
                ObterStatusAnaliseCredito(CPF_ERRO_COMUNICACAO);
            Assert.AreEqual(
                StatusConsultaCredito.ErroComunicacao, status);
        }

        [TestMethod]
        public void TesteCPFSemPendencias()
        {
            StatusConsultaCredito status =
                ObterStatusAnaliseCredito(CPF_SEM_PENDENCIAS);
            Assert.AreEqual(
                StatusConsultaCredito.SemPendencias, status);
        }

        [TestMethod]
        public void TesteCPFInadimplente()
        {
            StatusConsultaCredito status =
                ObterStatusAnaliseCredito(CPF_INADIMPLENTE);
            Assert.AreEqual(
                StatusConsultaCredito.Inadimplente, status);
        }
    }
}