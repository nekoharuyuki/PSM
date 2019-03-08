-- Variables

TitleString = "Lua Sample"

ColorList = {0xffff0000, 0xff00ff00,0xff0000ff,0xffffff00}
ColorIndex = 1

-- Functions

function OnTouch(pointX, pointY, isDown)
    print(string.format("%d %d %s", pointX, pointY, tostring(isDown)))

    if isDown then
        ColorIndex = ColorIndex + 1
    end
    if ColorIndex > #ColorList then
        ColorIndex = 1
    end

    -- Call C# function
    FillCircle(ColorList[ColorIndex], pointX, pointY, 120)
end
