using System.Collections.Generic;

namespace ConsoleClientRest
{
    public class ConsultarTodosRegistros
    {
        private List<UsuarioEntity> consultarTodosUsuariosResult;

        public List<UsuarioEntity> ConsultarTodosUsuariosResult
        {
            get
            {
                return consultarTodosUsuariosResult;
            }

            set
            {
                consultarTodosUsuariosResult = value;
            }
        }
    }
}
