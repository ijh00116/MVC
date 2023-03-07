using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    public class ControllerUnitInGame:Character
    {
        public ViewUnit _view;
        public Transform target;

        float skilluseGlobalCoolTime = 2.0f;
        float currentskillGlobalCooltime = 0.0f;
        //유저의 데이터(cloud.user:여기에는 레벨만 일단 있고 player.unit에서 생성된 정보 가져다가 세팅할것임)
        public ControllerUnitInGame(Transform parent, CancellationTokenSource cts):base(cts)
        {
            if (_view == null)
            {
                _view = Object.Instantiate(InGameResourcesBundle.Loaded.unit, parent);
            }
            _view.transform.position = Battle.Field.currentMap.playerPos.position;
            _view.Init(this);
            _view.gameObject.SetActive(true);
            _state = new StateMachine<eActorState>( true, cts);

            var idlestate = new CharacterIdle(this);
            _state.AddState(idlestate.GetState(), idlestate);
            var movestate = new CharacterMove(this);
            _state.AddState(movestate.GetState(), movestate);
            var attackstate = new CharacterAttack(this);
            _state.AddState(attackstate.GetState(), attackstate);

            //스킬
            var powerattackstate = new CharacterPowerAttackSkill(this);
            _state.AddState(powerattackstate.GetState(), powerattackstate);
            var explodeSkill= new CharacterExplodeSkill(this);
            _state.AddState(explodeSkill.GetState(), explodeSkill);
            var increaseatkSkill = new CharacterIncreaseAtkValueSkill(this);
            _state.AddState(increaseatkSkill.GetState(), increaseatkSkill);
            var petmissiletkSkill = new CharacterShootMissileSkill(this);
            _state.AddState(petmissiletkSkill.GetState(), petmissiletkSkill);
            var shootwaveSkill = new CharacterShootWaveFrontBack(this);
            _state.AddState(shootwaveSkill.GetState(), shootwaveSkill);
            var installTrap = new CharacterinstallTrap(this);
            _state.AddState(installTrap.GetState(), installTrap);
            var alldmg = new CharacterAllDamage(this);
            _state.AddState(alldmg.GetState(), alldmg);
            //스킬

            _state.ChangeState(eActorState.Idle);

            _state.StateStop(false);
            
            Main().Forget();
            //CameraUpdate().Forget();
            Player.Unit.userUnit = this;
            Battle.Field.UnitStop += UnitStopCallback;
            Battle.Field.UnitRestart += UnitRestartCallback;
        }
   
        async UniTaskVoid Main()
        {
            while (true)
            {
                RaycastHit2D hitright = Physics2D.Raycast(_view.raycastTr.position, Vector2.right, 2,_view.targetLayer);
                RaycastHit2D hitleft = Physics2D.Raycast(_view.raycastTr.position, Vector2.right, 2, _view.targetLayer);
                Debug.DrawRay(_view.raycastTr.position, Vector2.right*2, Color.red);
                Debug.DrawRay(_view.raycastTr.position, Vector2.left * 2, Color.red);
                if (hitright.collider != null)
                {
                    target = hitright.collider.gameObject.transform;
                    if (_state.IsCurrentState(eActorState.Move)|| _state.IsCurrentState(eActorState.Idle))
                    {
                        _state.ChangeState(eActorState.Attack);
                    }
                }
                else if (hitleft.collider != null)
                {
                    target = hitleft.collider.gameObject.transform;
                    if (_state.IsCurrentState(eActorState.Move) || _state.IsCurrentState(eActorState.Idle))
                    {
                        _state.ChangeState(eActorState.Attack);
                    }
                }
                else
                {
                    if (_state.IsCurrentState(eActorState.Move) || _state.IsCurrentState(eActorState.Idle)|| _state.IsCurrentState(eActorState.Attack))
                    {
                        //이동의 상태변환이 스킬상태변환을 막음
                        var enemy = Battle.Enemy.GetClosedEnemyController(_view.transform);
                        if (enemy!=null)
                        {
                            target = enemy._view.transform;
                            _state.ChangeState(eActorState.Move);
                        }
                        else
                        {
                            _state.ChangeState(eActorState.Idle);
                        }
                    }
                  
                }
         

                currentskillGlobalCooltime += Time.deltaTime;
                //등록된 스킬 사용
                var indexes = Player.Skill.GetAllRegisterSkillInput();
                Definition.SkillKey _skillhashkey=Definition.SkillKey.None;
                foreach (var index in indexes)
                {
                    var catkey = (Definition.SkillKey)index;
                    if(catkey!=Definition.SkillKey.None)
                    {
                        _skillhashkey = catkey;
                        break;
                    }  
                }
                if(_skillhashkey!=Definition.SkillKey.None && currentskillGlobalCooltime>=skilluseGlobalCoolTime)
                {
                    currentskillGlobalCooltime = 0.0f;
                    switch (_skillhashkey)
                    {
                        case Definition.SkillKey.None:
                            break;
                        case Definition.SkillKey.PowerfulAttack:
                            _state.ChangeState(eActorState.PowerAttack);
                            break;
                        case Definition.SkillKey.Explode:
                            _state.ChangeState(eActorState.Explode);
                            break;
                        case Definition.SkillKey.AddMagicDmg:
                            break;
                        case Definition.SkillKey.skill_3://atk increase
                            _state.ChangeState(eActorState.skill_3);
                            break;
                        case Definition.SkillKey.skill_4://pet spawn
                            _state.ChangeState(eActorState.skill_4);
                            break;
                        case Definition.SkillKey.skill_5:
                            _state.ChangeState(eActorState.skill_5);
                            break;
                        case Definition.SkillKey.skill_6:
                            _state.ChangeState(eActorState.skill_6);
                            break;
                        case Definition.SkillKey.skill_7://wave shoot
                            _state.ChangeState(eActorState.skill_7);
                            break;
                        case Definition.SkillKey.skill_8:
                            _state.ChangeState(eActorState.skill_8);
                            break;
                        case Definition.SkillKey.skill_9:
                            _state.ChangeState(eActorState.skill_9);
                            break;
                        case Definition.SkillKey.skill_10:
                            _state.ChangeState(eActorState.skill_10);
                            break;
                        case Definition.SkillKey.skill_11:
                            _state.ChangeState(eActorState.skill_11);
                            break;
                        case Definition.SkillKey.skill_12:
                            _state.ChangeState(eActorState.skill_12);
                            break;
                        case Definition.SkillKey.skill_13:
                            _state.ChangeState(eActorState.skill_13);
                            break;
                        case Definition.SkillKey.skill_14:
                            break;
                        case Definition.SkillKey.skill_15:
                            break;
                        case Definition.SkillKey.skill_16:
                            break;
                        case Definition.SkillKey.skill_17:
                            break;
                        case Definition.SkillKey.skill_18:
                            break;
                        case Definition.SkillKey.skill_19:
                            break;
                        case Definition.SkillKey.skill_20:
                            break;
                        case Definition.SkillKey.skill_21:
                            break;
                        case Definition.SkillKey.skill_22:
                            break;
                        case Definition.SkillKey.skill_23:
                            break;
                        case Definition.SkillKey.skill_24:
                            break;
                        case Definition.SkillKey.skill_25:
                            break;
                        case Definition.SkillKey.skill_26:
                            break;
                        case Definition.SkillKey.skill_27:
                            break;
                        case Definition.SkillKey.skill_28:
                            break;
                        case Definition.SkillKey.skill_29:
                            break;
                        case Definition.SkillKey.End:
                            break;
                        default:
                            break;
                    }
                    var skillcache=Player.Skill.Get(_skillhashkey);
                    skillcache.userSkilldata.elapsedCooltime = 0;
                    skillcache.waitForuseSkill = false;
                    Player.Skill.RemoveSkillInput((int)_skillhashkey);
                }

                //스킬 등록
                var skillEquiplist = Player.Skill.currentSkillContainer();
                foreach(var skillkey in skillEquiplist)
                {
                    if (skillkey == Definition.SkillKey.None)
                        continue;
                    var skilldata = Player.Skill.Get(skillkey);
                    if (skilldata.IsEquiped && _state.stop == false)
                    {
                        if (skilldata.leftCooltime > 0)
                        {
                            skilldata.userSkilldata.elapsedCooltime += Time.deltaTime;
                        }
                        else
                        {
                            if (skilldata.waitForuseSkill == false && Player.Skill.IsAutoSkill)
                            {
                                if(skillkey==Definition.SkillKey.PowerfulAttack|| skillkey == Definition.SkillKey.Explode)
                                {
                                    if(_state.IsCurrentState(eActorState.Attack))
                                    {
                                        skilldata.waitForuseSkill = true;
                                        Player.Skill.RegisterSkillInput((int)skilldata.tabledataSkill.skillKey);
                                    }
                                }
                                else
                                {
                                    skilldata.waitForuseSkill = true;
                                    Player.Skill.RegisterSkillInput((int)skilldata.tabledataSkill.skillKey);
                                }
                                
                            }
                        }
                    }
                }

                await UniTask.Yield(_cts.Token);
                await UniTask.WaitUntil(() => Battle.Field.unitActivePause == false);
            }
        }

        void UnitStopCallback()
        {
            _view.animator.enabled = false;
            _state.ChangeState(eActorState.Idle);
            target = null;
        }
        void UnitRestartCallback()
        {
            _view.animator.enabled = true;
        }
    }

    #region attack
    public class CharacterAttack : CharacterState
    {
        ControllerUnitInGame _unit;
        int waitframe = 10;
        int playframe = 0;
        public CharacterAttack(ControllerUnitInGame unit)
        {
            _unit = unit;
            Player.Unit.CallbackAtAttack += Attack;
        }

        public virtual void Attack()
        {
            var enemy = _unit.target.GetComponent<ViewEnemy>();
            if(enemy!=null)
            {
                var enemycon = Battle.Enemy.GetHashEnemyController(enemy.hash);
                double dmg = Player.Unit.Atk;

                float randomcri = Random.Range(0, 100);
                if(randomcri<=Player.Unit.CriRate)
                {
                    dmg *= Player.Unit.CriDmg;
                }
                enemycon.hp -= (float)dmg;

                WorldUIManager.Instance.InstatiateFont(enemy.transform.position, dmg, false,Color.white);
            }
        }

        public override eActorState GetState()
        {
            return eActorState.Attack;
        }

        protected override void OnEnter()
        {
            float atkSpeed = (float)Player.Unit.AtkSpeed;
            _unit._view.animator.SetFloat("AnimSpeed", (float)Player.Unit.AtkSpeed);
            _unit._view.animator.Play("Attack");
            playframe = 0;
        }

        protected override void OnExit()
        {
        }
        protected override void OnUpdate()
        {
            
        }
    }
    #endregion
    #region idle

    public class CharacterIdle : CharacterState
    {
        ControllerUnitInGame _unit;
        public CharacterIdle(ControllerUnitInGame unit)
        {
            _unit = unit;
        }
        protected override void OnEnter()
        {
            _unit._view.animator.SetFloat("AnimSpeed",1);
            _unit._view.animator.Play("Idle");
        }

        protected override void OnExit()
        {
        }

        protected override void OnUpdate()
        {
          
        }
        public override eActorState GetState()
        {
            return eActorState.Idle;
        }
    }
    #endregion
    #region move

    public class CharacterMove : CharacterState
    {
        ControllerUnitInGame _unit;
        Vector2 targetposition;
        public CharacterMove(ControllerUnitInGame unit)
        {
            _unit = unit;
        }
        protected override void OnEnter()
        {
            if(_unit._state.PreviousState!=eActorState.Move)
            {
                _unit._view.animator.SetFloat("AnimSpeed", (float)Player.Unit.MoveSpeed);
                _unit._view.animator.Play("Move");
            }
            else
            {
                Debug.Log("이동애님 재생 안해!");
            }
        }

        protected override void OnExit()
        {
        }

        protected override void OnUpdate()
        {
            targetposition = _unit.target.position;
            Vector2 dir = (targetposition - (Vector2)_unit._view.transform.position).normalized;

            float localscalex = Mathf.Abs(_unit._view.transform.localScale.x);
            float x = localscalex;
            if (dir.x >= 0)
            {
                x = -localscalex;
            }
            else
            {
                x = localscalex;
            }
            _unit._view.transform.localScale = new Vector2(x, _unit._view.transform.localScale.y);

            double movespeed = Player.Unit.MoveSpeed;
            _unit._view.transform.Translate(dir * Time.deltaTime * (float)movespeed * 8);

            
        }
        public override eActorState GetState()
        {
            return eActorState.Move;
        }
    }
    #endregion

}
