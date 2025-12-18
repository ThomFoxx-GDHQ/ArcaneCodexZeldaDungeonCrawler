using UnityEngine;

public class TimeOfDayManager : MonoSingleton<TimeOfDayManager>
{

    [SerializeField] FoxxTime _timeOfDay;
    [SerializeField] Light _directionalLight;
    [SerializeField] FoxxTime _sunRise, _sunSet;
    [SerializeField] Color _daylight, _twilight, _night;
    int _numberOfDays = 0;
    FoxxTime _previousTime;
    
    public int GetDays => _numberOfDays;

    public void ChangeTime(FoxxTime time)
    {
        _previousTime = _timeOfDay;
        _timeOfDay = time;
        CheckTime();
        if (_timeOfDay < _previousTime) _numberOfDays++;
    }

    [ContextMenu("CheckTime")]
    private void CheckTime()
    {
      /*  if (_timeOfDay < _sunRise)
            _directionalLight.intensity = .5f;
        else if (_timeOfDay > _sunSet)
            _directionalLight.intensity = .5f;
        else _directionalLight.intensity = 2;*/

        switch (_timeOfDay)
        {
            case FoxxTime x when x < _sunRise:
                _directionalLight.intensity = .5f;
                _directionalLight.color = _twilight;
                Debug.Log($"Time is now {_timeOfDay.whatTime}");
                break;
            case FoxxTime x when x > _sunSet:
                _directionalLight.intensity = .5f;
                _directionalLight.color = _night;
                Debug.Log($"Time is now {_timeOfDay.whatTime}");
                break;
            default:
                _directionalLight.intensity = 2;
                _directionalLight.color = _daylight;
                Debug.Log($"Time is now {_timeOfDay.whatTime}");
                break;
        }
    }

    public void AdvanceTime(int minutes)
    {
        if ( minutes < 0 )
        {
            Debug.LogWarning("No Backwards Time Travel, Marty!");
            return;
        }

        FoxxTime advance = new FoxxTime(0, 0);
        if ( minutes >= 60 )
        {   //pass in 135 minute
            advance.hour = minutes / 60; //hour = 2
            advance.minute = minutes % 60; //minutes =15
        }

        _previousTime = _timeOfDay;
        /*_timeOfDay.hour += advance.hour;
        _timeOfDay.minute += advance.minute;*/
        _timeOfDay += advance;

        if ( _timeOfDay.minute > 60 )
        {
            _timeOfDay.hour++;
            _timeOfDay.minute -= 60;
        }
        if ( _timeOfDay.hour >= 24 )
        {
            _timeOfDay.hour -= 24;
        }

        if (_timeOfDay < _previousTime)
            _numberOfDays++;
    }
}
