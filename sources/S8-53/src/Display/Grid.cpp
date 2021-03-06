#include "Grid.h"
#include "Settings/Settings.h"


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
int Grid::Left(void)
{
    return (MenuIsMinimize() && MenuIsShown() ? 9 : 20) + Measure_GetDeltaGridLeft();
}

//----------------------------------------------------------------------------------------------------------------------------------------------------
int Grid::Right(void)
{
    return ((MenuIsMinimize() && MenuIsShown()) ? 9 : 20) + 280;
}

//----------------------------------------------------------------------------------------------------------------------------------------------------
int Grid::ChannelBottom(void)
{
    return (sDisplay_IsSeparate()) ? (GRID_TOP + GRID_HEIGHT / 2) : FullBottom();
}

//----------------------------------------------------------------------------------------------------------------------------------------------------
int Grid::SignalWidth(void)
{
    return Width();
}

//----------------------------------------------------------------------------------------------------------------------------------------------------
int Grid::ChannelHeight(void)
{
    return (sDisplay_IsSeparate()) ? FullHeight() / 2 : FullHeight();
}

//----------------------------------------------------------------------------------------------------------------------------------------------------
int Grid::ChannelCenterHeight(void)
{
    return (GRID_TOP + ChannelBottom()) / 2;
}

//----------------------------------------------------------------------------------------------------------------------------------------------------
int Grid::FullBottom(void)
{
    return GRID_BOTTOM - Measure_GetDeltaGridBottom();
}

//----------------------------------------------------------------------------------------------------------------------------------------------------
int Grid::FullHeight(void)
{
    return FullBottom() - GRID_TOP;
}

//----------------------------------------------------------------------------------------------------------------------------------------------------
int Grid::Width(void)
{
    return Right() - Left();
}

//----------------------------------------------------------------------------------------------------------------------------------------------------
int Grid::FullCenterHeight(void)
{
    return (FullBottom() + GRID_TOP) / 2;
}

//----------------------------------------------------------------------------------------------------------------------------------------------------
int Grid::WidthInCells(void)
{
    return MenuIsShown() ? 10 : 14;
}

//----------------------------------------------------------------------------------------------------------------------------------------------------
float Grid::DeltaY(void)
{
    float delta = (FullBottom() - GRID_TOP) / 10.0f;
    return sDisplay_IsSeparate() ? (delta / 2.0f) : delta;
}

//----------------------------------------------------------------------------------------------------------------------------------------------------
float Grid::DeltaX(void)
{
    float delta = (Right() - Left()) / 14.0f;
    return delta;
}

//----------------------------------------------------------------------------------------------------------------------------------------------------
int Grid::MathTop(void)
{
    return MathBottom() - MathHeight();
}

//----------------------------------------------------------------------------------------------------------------------------------------------------
int Grid::MathHeight(void)
{
    if (ENABLED_FFT || MODE_DRAW_MATH_IS_SEPARATE)
    {
        return FullHeight() / 2;
    }
    return FullHeight();
}

//----------------------------------------------------------------------------------------------------------------------------------------------------
int Grid::MathBottom(void)
{
    return FullBottom();
}

//----------------------------------------------------------------------------------------------------------------------------------------------------
int Grid::BottomMessages(void)
{
    int retValue = FullBottom();
    if (MODE_WORK_IS_MEMINT)
    {
        retValue -= 12;
    }
    else if (SHOW_MEASURES)
    {
        retValue = TOP_MEASURES;
        if (MEAS_NUM_IS_6_1 || MEAS_NUM_IS_6_2 || MEAS_NUM_IS_1)
        {
            retValue = FullBottom();
        }
    }
    return retValue - 12;;
}
