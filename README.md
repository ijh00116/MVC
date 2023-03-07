# MVP
mvp에 대한 간략한 설명 및 현재 진행중인 프로젝트

## 내용
- Model에 테이블데이터와 유저 저장정보를 담아둡니다.model은 presenter에 정보를 전달하고 presenter는 모델의 데이터를 변경합니다.
- Presenter(애셋의 Controller폴더)는 뷰어,Model을 양방향 통신합니다.
- View는 Presenter와 양방향 통신하며 보여줍니다.

## 이점
- Model과 Presenter는 Monobehaviour의 상속을 피하고 비동기함수로 매 프레임마다 update를 돌려 최적화에 도움이 됩니다.
- Viewer만 Monobehaviour의 상속을 받아 렌더링을 맡습니다.
- 각각 역할에 맡게 구조화 하여 추후 추가,수정작업이 원활하도록 합니다.
