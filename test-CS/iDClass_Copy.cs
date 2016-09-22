using Controlid;
using Controlid.iDClass;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepTestAPI
{
    [TestClass]
    public class iDClass_Copy
    {

        /// <summary>
        /// Exemplo de rotina que copia os usuários de um REP para outro
        /// (Este exemplo é exclusivo para iDClass)
        /// </summary>
        [TestMethod, TestCategory("Rep iDClass")]
        public void CopyAll()
        {
            try
            {
                // Para simplificar os dados de login serão os mesmos, o que mudará logico será o IP do REP!
                ConnectRequest login = new ConnectRequest()
                {
                    Login = "admin",
                    Password = "admin"
                };

                // Conecta-se com o primeiro REP
                var REP1 = "192.168.0.19";
                var cn1 = RestJSON.SendJson<ConnectResult>(REP1, login); // Origem
                if (!cn1.isOK)
                    Assert.Inconclusive(REP1 + ": " + cn1.Status);

                // Conecta-se com o segundo REP
                var REP2 = "192.168.2.105";
                var cn2 = RestJSON.SendJson<ConnectResult>(REP2, login); // Destino
                if (!cn2.isOK)
                    Assert.Inconclusive(REP2 + ": " + cn2.Status);

                // Este comando requisita somente os 10 primeiros registros (apenas para evitar erros de comunicação quando há muitos dados)
                // para simplificar este exemplo não será feito nenhum loop ou tratamento, isso será abordado no exemplo seguinte
                // Estando ambos os REP Conectados, primeiro lê todos os usuários do REP1
                var userBlock = RestJSON.SendJson<UserResult>(REP1, new UserRequest() { Limit = 10, Templates=true }, cn1.Session);

                // É gerado um arquivo "log-RepCID-iDClass.txt" na pasta onde fica a DLL que contem toda a comunicação
                // Mas é possivel desabilitar o log de comunicação, por padrão vem habilitado por ser é bem util no desenvolvimento ou para encontrar problemas
                // RestJSON.WriteLog = false; 
                // Evite manter isso ligado em produção para não acabar com o espaço em disco, pois esse log fica realmente muito grande
                // RestJSON.TimeOut = 30000;
                // É possivel também reconfigurar o timeout padrão das requisição

                // ATENÇÃO: por segurança só existe substituição de dados quando isso é feita de forma explicita!
                // Portanto os comandos de INSERT e UPDATE são distintos, e cada um tem validações específicas
                // var insertRequest = new UserAddRequest() { Usuario = userBlock.ListUsers }; // Não é possivel inserir pessoas com o mesmo PIS
                var modifyRequest = new UserUpdateRequest() { Usuario = userBlock.ListUsers }; // Não é possivel alterar pessoas que não existem (baseado no PIS, que é sempre o identificador único)

                var result = RestJSON.SendJson<StatusResult>(REP2, modifyRequest, cn2.Session);
                if (result.isOK)
                    Console.Write(result.Status);
                else
                    Assert.Inconclusive(result.Status);
            }
            catch(Exception ex)
            {
                // Erro desconhecido (qualquer erro que não vier do REP, incluindo erros de rede ou comunicação)
                Assert.Fail(ex.Message);
            }
        }
    }
}