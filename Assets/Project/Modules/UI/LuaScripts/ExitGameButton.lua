-- 退出游戏按钮

local ButtonClass = CS.UnityEngine.UI.Button
local EventBusClass = CS.GameFramework.Core.EventBus

-- 发送退出游戏请求
function SendExitGameRequest()
    local eventBus = EventBusClass.Instance
    eventBus:Publish(ExitGameEvent)
end

-- 初始化函数
function Init(btnGameObject)
    if not btnGameObject then
        CS.UnityEngine.Debug.Log("错误：未传入“退出游戏”按钮GameObject。")
        return
    end

    -- 获取按钮上Button组件
    ButtonComponent = btnGameObject:GetComponent(typeof(ButtonClass))
    if ButtonComponent then
        -- 清空原有监听
        ButtonComponent.onClick:RemoveAllListeners()
        -- 添加监听器
        ButtonComponent.onClick:AddListener(
            function ()
                SendExitGameRequest()
            end
        )
        CS.UnityEngine.Debug.Log("成功：已在“退出游戏按钮”上添加按钮监听器。")
    else
        CS.UnityEngine.Debug.Log("成功：已在“退出游戏按钮”上添加按钮监听器。")
    end
end