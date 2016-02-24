﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Controlid;
using Controlid.iDClass;
using System.Threading;

namespace RepTestAPI
{
    [TestClass]
    public class Config
    {
        static RepCid rep;

        //public static readonly string repIP = "192.168.1.102"; // iDX
        public static readonly string repIP = "192.168.0.19"; // iDClass
        //public static readonly int repPort = 1818; // iDX
        public static readonly int repPort = 443; // iDClass
        public static readonly string repLogin = "admin";
        public static readonly string repSenha = "admin";
        public static readonly uint repiDXSenha = 0;

        // PIS: Fabio Ferreira
        public static Int64 pisTEST = 012468202319; 

        public static RepCid ConectarREP()
        {
            if (rep == null)
            {
                rep = new RepCid();
                rep.iDClassLogin = repLogin;
                rep.iDClassPassword = repSenha;
                Controlid.RepCid.ErrosRep status = rep.Conectar(repIP, repPort, repiDXSenha);
                if (status == RepCid.ErrosRep.OK)
                    Console.WriteLine("REP Conectado");
                else
                {
                    Console.WriteLine(rep.LastLog());
                    Assert.Fail("Erro ao conectar: " + status.ToString());
                }
            }
            return rep;
        }

        [TestInitialize]
        public void Conectar()
        {
            ConectarREP();
        }

        [TestMethod, TestCategory("RepCid")]
        public void Config_GetInfo()
        {
            string sn;
            uint tam_bobina, restante_bobina, uptime, cortes, papel_acumulado, nsr_atual;
            if (rep.LerInfo(out sn, out tam_bobina, out restante_bobina, out uptime, out cortes, out papel_acumulado, out nsr_atual))
            {
                Console.WriteLine("sn: " + sn);
                Console.WriteLine("tam_bobina: " + tam_bobina);
                Console.WriteLine("restante_bobina: " + restante_bobina);
                Console.WriteLine("uptime: " + uptime);
                Console.WriteLine("cortes: " + cortes);
                Console.WriteLine("papel_acumulado: " + papel_acumulado);
                Console.WriteLine("nsr_atual: " + nsr_atual);
            }
            else
                Assert.Fail("Não foi possivel ler as informações do REP");
        }

        [TestMethod, TestCategory("RepCid")]
        public void Config_GetDateTime()
        {
            DateTime dt = rep.LerDataHora();
            Console.WriteLine(string.Format("{0:dd/MM/yyyy HH:mm:ss}", dt));
        }

        [TestMethod, TestCategory("RepCid")]
        public void Config_SetDateTime()
        {
            // Assert.IsTrue(rep.GravarDataHora(DateTime.Now.AddDays(-1).AddMinutes(-33)), "Erro ao gravar Data/Hora no REP");
            // Vale lembrar que dependendo do teste a sessão pode expirar se o horario mudar mais de 4 horas de diferença
            Assert.IsTrue(rep.GravarDataHora(DateTime.Now), "Erro ao gravar Data/Hora no REP");
        }

        [TestMethod, TestCategory("RepCid")]
        public void Config_GetHorarioVerao()
        {
            int iAno, iMes, iDia;
            int fAno, fMes, fDia;
            if (rep.LerConfigHVerao(out iAno, out iMes, out iDia, out fAno, out fMes, out fDia))
            {
                Console.WriteLine(string.Format("Inicio: {0:D2}/{1:D2}/{2:D4}", iDia, iMes, iAno));
                Console.WriteLine(string.Format("Fim: {0:D2}/{1:D2}/{2:D4}", fDia, fMes, fAno));
            }
            else
            {
                Console.WriteLine(rep.LastLog());
                Assert.Fail("Erro ao Ler Horário de Verão");
            }
        }

        [TestMethod, TestCategory("RepCid")]
        public void Config_SetHorarioVerao()
        {
            bool gravou;
            if (rep.GravarConfigHVerao(2017, 6, 5, 2018, 7, 6, out gravou) && gravou)
                Console.WriteLine("Horário de Verão gravado");
            else
            {
                Console.WriteLine(rep.LastLog());
                Assert.Fail("Erro ao Ler Horário  de Verão");
            }
        }

        [TestMethod, TestCategory("RepCid")]
        public void Config_ApagarAdmins()
        {
            bool lOk;
            if (!(rep.ApagarAdmins(out lOk) && lOk))
            {
                Console.WriteLine(rep.LastLog());
                Assert.Fail("Erro ao apagar administradores");
            }
            Console.WriteLine("Administradores Excluido");
        }

        [TestMethod, TestCategory("RepCid")]
        public void Config_NetworkGet()
        {
            string ip;
            string netmask;
            string gateway;
            UInt16 porta;
            if (rep.LerConfigRede(out ip, out netmask, out gateway, out porta))
            {
                Console.WriteLine("IP: " + ip);
                Console.WriteLine("Netmask: " + netmask);
                Console.WriteLine("Gateway: " + gateway);
                Console.WriteLine("Porta: " + porta);
            }
            else
                Assert.Fail("Erro ao ler configurações de rede");
        }

        [TestMethod, TestCategory("RepCid")]
        public void Config_NetworkSet()
        {
            /*
            
            bool gravou;
            string ip = repIP; // "192.168.0.147";
            ushort port = (ushort)repPort;
            ushort newPort = (ushort)(repPort + 1);

            // ATENÇÃO: se este teste falhar parcialmente, todos os demais irão falhar também.
            // portanto deve ser o ultimo teste a ser executado
            if (rep.GravarConfigRede(ip, "255.255.0.0", "192.168.0.1", newPort, out gravou) && gravou)
            {
                Console.WriteLine("Nova configuração gravada");
                rep.Desconectar();
                Thread.Sleep(ip == repIP ? 5000 : 30000); // tempo para o ip mudar...
                if (rep.Conectar(ip, newPort) == RepCid.ErrosRep.OK)
                {
                    Console.WriteLine("Conectado ao novo IP");
                    if (rep.GravarConfigRede(repIP, "255.255.128.0", "192.168.0.1", port, out gravou) && gravou)
                    {
                        Thread.Sleep(ip == repIP ? 5000 : 30000); // tempo para o ip mudar...
                        Console.WriteLine("Voltou ao IP padrão");
                    }
                    else
                        Assert.Fail("Erro ao gravar configurações antiga de rede");
                }
                else
                    Assert.Fail("Erro ao conectar o REP no novo IP:" + ip);
            }
            else
                Assert.Fail("Erro ao mudar configuração de rede");
            
            */
        }
    }
}
