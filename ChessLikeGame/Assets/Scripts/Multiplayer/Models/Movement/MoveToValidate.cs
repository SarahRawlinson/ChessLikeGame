using System.Collections.Generic;

namespace Multiplayer.Models.Movement
{
    public class MoveToValidate
    {
        private MoveTypes _moveType;
        private List<MoveValidationTypes> _validators;
        private int _numberOfSteps;

        public MoveToValidate(MoveTypes moveType, List<MoveValidationTypes> validators, int numberOfSteps)
        {
            _moveType = moveType;
            _validators = validators;
            _numberOfSteps = numberOfSteps;
        }

        public MoveTypes MoveType => _moveType;
        public List<MoveValidationTypes> Validators => _validators;
        public int NumberOfSteps => _numberOfSteps;
    }
}