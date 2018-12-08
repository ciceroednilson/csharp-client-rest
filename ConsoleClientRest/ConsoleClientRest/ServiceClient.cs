using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleClientRest
{

    public class ServiceClient
    {
        /// <summary>
        /// URL DO NOSSO SERVIÇO REST
        /// </summary>
        private string url = "http://localhost:61263/ServiceUsuario.svc";

        /// <summary>
        /// TIPO DO CONTEUDO QUE VAMOS USAR 
        /// </summary>
        private string contentType = "application/json; charset=utf-8";

        /// <summary>
        /// OBJETO PARA REALIZAR REQUISIÇÕES
        /// </summary>
        private System.Net.WebRequest webRequest;

        /// <summary>
        /// PREPARA O NOSSO OBJETO DE REQUISIÇÕES PARA CADA OPERAÇÃO
        /// </summary>
        /// <param name="operacao"></param>
        /// <param name="metodo"></param>
        private void PreparaWebRequest(string operacao, string metodo)
        {

            webRequest = System.Net.WebRequest.Create(url + "/" + operacao);
            webRequest.Method = metodo;
            webRequest.ContentType = this.contentType;
        }
        /// <summary>
        /// ESSE MÉTODO RETORNA UM Dictionary QUANDO O JSON PASSADO FOR UMA STRING COM CHAVE E VALOR
        /// ----------------------------------------------------------------------------------------
        /// EXEMPLO: "{\"InserirNovoRegistroResult\":\"Registro inserido com sucesso!\"}" 
        /// ----------------------------------------------------------------------------------------
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public Dictionary<string, string> DeserializeString(string json)
        {

            string[] resultado = json.Replace("{", string.Empty)
                                .Replace("}", string.Empty)
                                .Replace(@"\", string.Empty)
                                .Replace("\"", string.Empty).Split(':');

            Dictionary<string, string> saida = new Dictionary<string, string>();

            saida.Add(resultado[0], resultado[1]);

            return saida;
        }
        /// <summary>
        /// ESSE MÉTODO TORNAR UM OBJETO EM STRING JSON PARA ENVIARMOS PARA O SERVIÇO REST
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objeto"></param>
        /// <returns></returns>
        public string Serialize<T>(T objeto)
        {
            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(objeto.GetType());

            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();

            serializer.WriteObject(memoryStream, objeto);

            string stringJson = Encoding.UTF8.GetString(memoryStream.ToArray());

            return stringJson;
        }
        /// <summary>
        /// ESSE MÉTODO TRANSFORMA UMA STRING JSON EM UM OBJETO QUE É PASSADO NO TIPO GENÉRICO (T)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stringJson"></param>
        /// <returns></returns>
        public T Deserialize<T>(string stringJson)
        {
            T objeto = Activator.CreateInstance<T>();

            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(Encoding.Unicode.GetBytes(stringJson));

            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(objeto.GetType());

            objeto = (T)serializer.ReadObject(memoryStream);

            memoryStream.Close();

            return objeto;
        }

        /// <summary>
        /// ESSE MÉTODO VAI CHAMAR A OPERAÇÃO PARA INSERIR UM NOVO REGISTRO DO SERVIÇO REST
        /// </summary>
        /// <param name="usuarioEntity"></param>
        /// <returns></returns>
        public string InserirNovoRegistro(UsuarioEntity usuarioEntity)
        {
            PreparaWebRequest("InserirNovoRegistro", "POST");

            InserirRegistroRequest inserirRegistroRequest = new InserirRegistroRequest();
            inserirRegistroRequest.usuarioEntity = usuarioEntity;

            string jsonSerialize = this.Serialize<InserirRegistroRequest>(inserirRegistroRequest);

            using (var streamWriter = new System.IO.StreamWriter(webRequest.GetRequestStream()))
            {
                streamWriter.Write(jsonSerialize);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var response = (System.Net.HttpWebResponse)webRequest.GetResponse();


            string resultadoJson = null;

            using (var streamReader = new System.IO.StreamReader(response.GetResponseStream()))
                resultadoJson = streamReader.ReadToEnd();


            return DeserializeString(resultadoJson).Values.First();

        }
        /// <summary>
        /// ESSE MÉTODO VAI CONSULTAR UM ÚNICO REGISTRO PELO SEU CÓDIGO(ID)
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public UsuarioEntity ConsultarRegistroPorCodigo(int codigo)
        {
            ConsultarRegistroPorCodigo usuario = new ConsultarRegistroPorCodigo();

            string resultadoJson = "";

            PreparaWebRequest("ConsultarRegistroPorCodigo/" + codigo, "GET");

            var response = (System.Net.HttpWebResponse)webRequest.GetResponse();

            using (var streamReader = new System.IO.StreamReader(response.GetResponseStream()))
            {

                resultadoJson = streamReader.ReadToEnd();

                usuario = Deserialize<ConsultarRegistroPorCodigo>(resultadoJson);
            }

            return usuario.ConsultarRegistroPorCodigoResult;
        }
        /// <summary>
        /// ESSE MÉTODO VAI RETORNAR TODOS OS REGISTROS CADASTRADOS
        /// </summary>
        /// <returns></returns>
        public List<UsuarioEntity> ConsultarUsuarios()
        {
            ConsultarTodosRegistros registros = new ConsultarTodosRegistros();

            string resultadoJson = "";

            PreparaWebRequest("ConsultarTodosUsuarios", "GET");

            var response = (System.Net.HttpWebResponse)webRequest.GetResponse();

            using (var sr = new System.IO.StreamReader(response.GetResponseStream()))
            {

                resultadoJson = sr.ReadToEnd();

                registros = Deserialize<ConsultarTodosRegistros>(resultadoJson);
            }

            return registros.ConsultarTodosUsuariosResult;
        }

        /// <summary>
        /// ESSE MÉTODO VAI ATUALIZAR O REGISTRO INFORMADO
        /// </summary>
        /// <param name="usuarioEntity"></param>
        /// <returns></returns>
        public string AtualizarRegistro(UsuarioEntity usuarioEntity)
        {
            PreparaWebRequest("AtualizarRegistro", "PUT");

            string jsonSerialize = this.Serialize<UsuarioEntity>(usuarioEntity);

            using (var streamWriter = new System.IO.StreamWriter(webRequest.GetRequestStream()))
            {
                streamWriter.Write(jsonSerialize);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var response = (System.Net.HttpWebResponse)webRequest.GetResponse();

            string resultadoJson = null;

            using (var streamReader = new System.IO.StreamReader(response.GetResponseStream()))
                resultadoJson = streamReader.ReadToEnd();


            return resultadoJson.Replace(@"\", string.Empty).Replace("\"", string.Empty);

        }
        /// <summary>
        /// ESSE MÉTODO VAI EXCLUIR UM REGISTRO PELO CÓDIGO
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public string ExcluirUsuario(int codigo)
        {

            PreparaWebRequest("ExcluirUsuario/" + codigo, "DELETE");

            var response = (System.Net.HttpWebResponse)webRequest.GetResponse();

            string resultadoJson = null;
            using (var streamReader = new System.IO.StreamReader(response.GetResponseStream()))
            {
                resultadoJson = streamReader.ReadToEnd();
            }

            return resultadoJson.Replace(@"\", string.Empty).Replace("\"", string.Empty);
        }


    }
}

