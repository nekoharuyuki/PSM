luanet.load_assembly('System')
luanet.load_assembly('Sce.PlayStation.Core')
luanet.load_assembly('SampleLib')

GraphicsContext = luanet.import_type "Sce.PlayStation.Core.Graphics.GraphicsContext"
ShaderProgram = luanet.import_type "Sce.PlayStation.Core.Graphics.ShaderProgram"
VertexBuffer = luanet.import_type "Sce.PlayStation.Core.Graphics.VertexBuffer"
VertexFormat = luanet.import_type "Sce.PlayStation.Core.Graphics.VertexFormat"
Texture2D = luanet.import_type "Sce.PlayStation.Core.Graphics.Texture2D"
DrawMode = luanet.import_type "Sce.PlayStation.Core.Graphics.DrawMode"
Event = luanet.import_type "Sce.PlayStation.Core.Environment.SystemEvents"
FMath = luanet.import_type "Sce.PlayStation.Core.FMath"
Matrix4 = luanet.import_type "Sce.PlayStation.Core.Matrix4"
Vector3 = luanet.import_type "Sce.PlayStation.Core.Vector3"
Stopwatch = luanet.import_type "System.Diagnostics.Stopwatch"
SampleDraw = luanet.import_type "Sample.SampleDraw"
Float = luanet.import_type "System.Single"

local graphics
local stopwatch
local program
local vbuffer
local texture

local Texture2D_FromFile
local graphics_SetViewport
local graphics_SetClearColor
local vbuffer_SetVertices
local SampleDraw_DrawText

function Main()
    Init()
    while true do
        Event.CheckEvents()
        Update()
        Render()
    end
    Term()
end

function Init()
    graphics = GraphicsContext()
    stopwatch = Stopwatch()
    stopwatch:Start()

    SampleDraw.Init(graphics)

    -- Overloaded constructors
    Texture2D_FromFile = luanet.get_constructor_bysig(Texture2D, "System.String", "System.Boolean")

    program = ShaderProgram("/Application/shaders/VertexColor.cgx")
    vbuffer = VertexBuffer(3, {VertexFormat.Float3, VertexFormat.Float2, VertexFormat.Float4})
    texture = Texture2D_FromFile("/Application/test.png", false)

    -- Overloaded methods
    graphics_SetViewport = luanet.get_method_bysig(graphics, "SetViewport", "System.Int32", "System.Int32", "System.Int32", "System.Int32")
    graphics_SetClearColor = luanet.get_method_bysig(graphics, "SetClearColor", "System.Single", "System.Single", "System.Single", "System.Single")
    vbuffer_SetVertices = luanet.get_method_bysig(vbuffer, "SetVertices", "System.Int32", "System.Array")
    SampleDraw_DrawText = luanet.get_method_bysig(SampleDraw, "DrawText", "System.String", "System.UInt32", "System.Int32", "System.Int32")

    program:SetUniformBinding(0, "WorldViewProj")
    program:SetAttributeBinding(0, "a_Position")
    program:SetAttributeBinding(1, "a_TexCoord")

    positions = Float[9] 
    positions[0] = 0.0      positions[1] = 0.577        positions[2] = 0.0
    positions[3] = -0.5     positions[4] = -0.289       positions[5] = 0.0
    positions[6] = 0.5      positions[7] = -0.289       positions[8] = 0.0

    texcoords = Float[6]
    texcoords[0] = 0.5      texcoords[1] = 0.0
    texcoords[2] = 0.0      texcoords[3] = 1.0
    texcoords[4] = 1.0      texcoords[5] = 1.0

    colors = Float[12]
    colors[0] = 1.0     colors[1] = 0.0     colors[2] = 0.0     colors[3] = 1.0
    colors[4] = 0.0     colors[5] = 1.0     colors[6] = 0.0     colors[7] = 1.0
    colors[8] = 0.0     colors[9] = 0.0     colors[10] = 1.0    colors[11] = 1.0

    vbuffer_SetVertices(0, positions)
    vbuffer_SetVertices(1, texcoords)
    vbuffer_SetVertices(2, colors)
end

function Term()
    SampleDraw.Term()
    program:Dispose()
    vbuffer:Dispose();
    texture:Dispose();
    graphics:Dispose();
end

function Update()
    SampleDraw.Update()
end

function Render()
    local seconds = stopwatch.ElapsedMilliseconds / 1000.0
    local aspect = graphics.Screen.AspectRatio
    local fov = FMath.Radians(45.0)

    local proj = Matrix4.Perspective(fov, aspect, 1.0, 1000000.0)
    local view = Matrix4.LookAt(Vector3(0.0, 0.0, 3.0),
                                Vector3(0.0, 0.0, 0.0),
                                Vector3.UnitY)
    local world = Matrix4.RotationY(1.0 * seconds)

    local worldViewProj = proj:Multiply(view:Multiply(world))
    program:SetUniformValue(0, worldViewProj)
                      
    graphics_SetViewport(0, 0, graphics.Screen.Width, graphics.Screen.Height)
    graphics_SetClearColor(0.0, 0.5, 1.0, 0.0)
    graphics:Clear()

    graphics:SetShaderProgram(program)
    graphics:SetVertexBuffer(0, vbuffer)
    graphics:SetTexture(0, texture)
    graphics:DrawArrays(DrawMode.TriangleStrip, 0, 3)

    SampleDraw_DrawText("Lua Triangle Sample", 0xffffffff, 0, 0)
    graphics:SwapBuffers()

    -- Gabage collection
    seconds = nil
    aspect = nil
    fov = nil
    proj = nil
    view = nil
    world = nil
    worldViewProj = nil
    collectgarbage()
end 
