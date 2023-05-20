using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Text.Json;

namespace MiniGameFramework
{

    public struct RCPostActResult
    {
        public string cmd;
        public object content;
        public object result;
    }

    public class RESTFulClient
    {
        protected struct PostAct
        {
            public string cmd;
            public object content;
            public Type contentType;
            public Type resultType;
            public Action<RCPostActResult> cb;
        }

        protected struct PostActResult
        {
            public RCPostActResult res;
            public Action<RCPostActResult> cb;
        }

        protected IHttpClient _httpClient;
        protected string _url;

        protected List<PostAct> _Acts;
        protected Queue<PostActResult> _ActResults;

        protected bool _finished;
        protected AutoResetEvent _actsEvent;

        protected async void _PostActRoutine()
        {
            Debug.DebugOutput(DebugTraceType.DTT_System, $"start RESTFul Client post act routine");

            while (false)
            {
                _actsEvent.WaitOne();

                PostAct[] actAry;
                lock(_Acts)
                {
                    actAry = _Acts.ToArray();
                }
                for(int i=0; i< actAry.Length; ++i)
                {
                    object retObj = null;
                    try
                    {
                        //string contentStr = JsonSerializer.Serialize(actAry[i].content, actAry[i].contentType);
                        string contentStr = JsonUtil.ToJson(actAry[i].content, actAry[i].contentType);
                        string cmdUrl = _url + actAry[i].cmd;

                        Debug.DebugOutput(DebugTraceType.DTT_Detail, $"requsting RESTFul API [{cmdUrl}] with content [{contentStr}]...");

                        string resStr = await _httpClient.PostJsonStringAsync(_url + actAry[i].cmd, contentStr);

                        Debug.DebugOutput(DebugTraceType.DTT_Detail, $"RESTFul API [{cmdUrl}] returns [{resStr}] ");

                        //retObj = JsonSerializer.Deserialize(resStr, actAry[i].resultType);
                        retObj = JsonUtil.FromJson(resStr, actAry[i].resultType);
                    }
                    catch(Exception e)
                    {
                        Debug.DebugOutput(DebugTraceType.DTT_Error, $"Post Act [{_url}][{actAry[i].cmd}] on err {e.Message}");
                        Debug.DebugOutput(DebugTraceType.DTT_Error, e.StackTrace);
                    }
                    
                    lock(_ActResults)
                    {
                        _Acts.RemoveAt(i);

                        _ActResults.Enqueue(new PostActResult()
                        {
                            res = new RCPostActResult()
                            {
                                cmd = actAry[i].cmd,
                                content = actAry[i].content,
                                result = retObj
                            },
                            cb = actAry[i].cb
                        });
                    }
                }

                if (_finished)
                {
                    // dispose after the last request
                    _httpClient.OnDispose();
                    break;
                }
            }

            Debug.DebugOutput(DebugTraceType.DTT_System, $"exit RESTFul Client post act routine");
        }

        public void DoAction<T, U>(string cmd, U content, Action<RCPostActResult> cb) where T : class
        {
            lock(_Acts)
            {
                _Acts.Add(new PostAct()
                {
                    cmd = cmd,
                    content = content,
                    cb = cb,
                    contentType = typeof(U),
                    resultType = typeof(T)
                });
            }

            _actsEvent.Set();
        }

        public void Init(string url)
        {
            Debug.DebugOutput(DebugTraceType.DTT_System, $"init restful client with url [{url}]");

            _url = url;
            _finished = false;
            _actsEvent = new AutoResetEvent(false);
            _httpClient = GameApp.Inst.Net.CreateHttpClient();

            _Acts = new List<PostAct>();
            _ActResults = new Queue<PostActResult>();
            
            Task.Run((Action)_PostActRoutine);
        }

        public void Fin()
        {
            _finished = true;

            _actsEvent.Set();
        }

        public void OnUpdate()
        {
            PostActResult[] resAry;
            lock (_ActResults)
            {
                resAry = _ActResults.ToArray();
            }

            if(resAry.Length <= 0)
            {
                return;
            }

            // call back
            for(int i=0; i< resAry.Length; ++i)
            {
                resAry[i].cb(resAry[i].res);
            }
        }
    }
}
