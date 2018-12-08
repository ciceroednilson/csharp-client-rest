
namespace ConsoleClientRest
{
    public class InserirRegistroRequest
    {
        private UsuarioEntity usuario;

        public UsuarioEntity usuarioEntity
        {
            get
            {
                return usuario;
            }

            set
            {
                usuario = value;
            }
        }
    }
}
