using System;

namespace LukaLukaModel.GUI.Controls.ModelView
{
    internal interface IGLDraw : IDisposable
    {
        void Draw( GLShaderProgram shaderProgram );
    }
}
